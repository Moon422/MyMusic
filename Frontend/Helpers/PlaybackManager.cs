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

    public PlaybackStatus PlaybackStatus { get; set; } = PlaybackStatus.Stopped;
    public int Volume { get; set; }
    public bool Muted { get; set; }
    public int CurentTrackId { get; set; }
    public TimeSpan TotalTime { get; set; }

    public async Task ChangeTrack(string filename = "AudioMainFile/Aalo_Tahsan.mp3")
    {
        if (PlaybackStatus != PlaybackStatus.Playing)
        {
            PlaybackStatus = PlaybackStatus.Loading;


            requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:100.0) Gecko/20100101 Firefox/100.0");
            var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var trackResponse = await response.Content.ReadFromJsonAsync<TrackBlobResponse>();
                CurentTrackId = await howl.Play(trackResponse!.Data);
            }
        }
    }
}
