namespace MyMusic.Frontend.Helpers;

public class PlaybackInfo
{
    public PlaybackStatus PlaybackStatus { get; set; } = PlaybackStatus.Stopped;
    public int Volume { get; set; }
    public bool Muted { get; set; }
    public int CurentTrackId { get; set; }
}
