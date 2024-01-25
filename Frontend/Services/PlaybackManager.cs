using Microsoft.JSInterop;

namespace MyMusic.Frontend.Services;

public class PlaybackManager
{


    // public bool Playing { get; set; } = false;
    // public bool Muted { get; set; } = false;
    // public int Volume { get; set; } = 100;


    // public void ChangeTrack(string filename)
    // {

    // }
    private IJSRuntime jsruntime;

    private bool playing = false;
    private bool muted = false;
    private int volume = 100;
    private string currentTrack = string.Empty;

    public PlaybackManager(IJSRuntime jsruntime)
    {
        this.jsruntime = jsruntime;
        this.jsruntime.InvokeVoidAsync("initializeAudioPlayer", playing, volume, muted);
    }

    public bool Playing
    {
        get => playing;
        set
        {
            playing = value;
            jsruntime.InvokeVoidAsync("setPlaying", playing);
        }
    }

    public bool Muted
    {
        get => muted;
        set
        {
            muted = value;
            jsruntime.InvokeVoidAsync("setMuted", muted);
        }
    }

    public void ChangeTrack(string filename)
    {

    }
}
