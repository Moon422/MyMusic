using Howler.Blazor.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyMusic.Frontend.Helpers;

namespace MyMusic.Frontend;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped<IHowl, Howl>();
        builder.Services.AddScoped<IHowlGlobal, HowlGlobal>();
        builder.Services.AddScoped<LoginManager>();
        builder.Services.AddScoped<PlaybackManager>();

        await builder.Build().RunAsync();
    }
}
