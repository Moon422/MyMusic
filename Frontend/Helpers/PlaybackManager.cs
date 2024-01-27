using System.Net.Http.Json;
using Howler.Blazor.Components;
using MyMusic.ViewModels;

namespace MyMusic.Frontend.Helpers;

public class PlaybackManager
{
    private const string BASE_URL = "https://coreapi.shadhinmusic.com/api/v5";

    private readonly IHowl howl;
    private readonly IHowlGlobal globalHowl;
    private readonly HttpClient httpClient;

    private int volume = 60;
    private HowlOptions howlOptions;

    public PlaybackManager(IHowl howl, IHowlGlobal globalHowl, HttpClient httpClient)
    {
        this.howl = howl;
        this.globalHowl = globalHowl;
        this.httpClient = httpClient;

        howlOptions = new HowlOptions { Volume = volume / 100d };
    }

    public PlaybackStatus PlaybackStatus { get; set; } = PlaybackStatus.Stopped;
    public Action StateHasChangedCallback { get; set; }
    public ReadTrackDto Track { get; set; }
    public int Volume
    {
        get => volume;
        set
        {
            volume = value;
            howlOptions.Volume = volume / 100d;
        }
    }
    public bool Muted { get; set; }
    public int CurentTrackId { get; set; }
    public TimeSpan TotalTime { get; set; }

    public async Task ChangeTrack(ReadTrackDto track)
    {
        if (PlaybackStatus != PlaybackStatus.Playing)
        {
            Track = track;
            TotalTime = Track.Duration;
            string filename = Track.TrackUrl;
            PlaybackStatus = PlaybackStatus.Loading;
            StateHasChangedCallback();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{BASE_URL}/streaming/getpth?ptype=S&type=null&ttype=null&name={filename}");
            requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:100.0) Gecko/20100101 Firefox/100.0");
            var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var trackResponse = await response.Content.ReadFromJsonAsync<TrackBlobResponse>();
                howlOptions.Sources = new[] { trackResponse!.Data };
                CurentTrackId = await howl.Play(howlOptions);
            }
            else
            {
                Console.WriteLine("Failed");
            }
        }
    }
}
