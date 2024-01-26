using System.Net.Http.Json;
using Howler.Blazor.Components;
using MyMusic.ViewModels;

namespace MyMusic.Frontend.Services;

public enum PlaybackState
{
    Playing,
    Paused,
    Stopped,
    Undefined
}

public class PlaybackManager
{
    private readonly IHowl howl;
    private readonly HttpClient httpClient;
    private const string BASE_URL = "https://coreapi.shadhinmusic.com/api/v5";

    public TimeSpan TotalTime { get; set; }
    public PlaybackState PlaybackState { get; set; } = PlaybackState.Undefined;
    public bool Muted { get; set; } = false;
    public int volume { get; set; } = 100;
    public int? CurrentTrackId { get; set; } = null;

    public PlaybackManager(IHowl howl, HttpClient httpClient)
    {
        this.howl = howl;
        this.httpClient = httpClient;
        Initialize();
    }

    private void Initialize()
    {
        howl.OnPlay += e =>
        {
            PlaybackState = PlaybackState.Playing;
        };

        howl.OnStop += e =>
        {
            PlaybackState = PlaybackState.Stopped;
        };

        howl.OnPause += e =>
        {
            PlaybackState = PlaybackState.Paused;
        };
    }

    public async void ChangeTrack(string filename = "AudioMainFile/Aalo_Tahsan.mp3")
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{BASE_URL}/streaming/getpth?ptype=S&type=null&ttype=null&name={filename}");
        requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:100.0) Gecko/20100101 Firefox/100.0");
        var response = await httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            var trackResponse = await response.Content.ReadFromJsonAsync<TrackBlobResponse>();
            CurrentTrackId = await howl.Play(trackResponse!.Data);
        }
    }
}
