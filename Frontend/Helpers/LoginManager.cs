using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using MyMusic.ViewModels;
using MyMusic.ViewModels.Enums;

namespace MyMusic.Frontend.Helpers;

public class LoginManager
{
    private readonly HttpClient httpClient;

    private LoginResponse? instance;
    private ArtistDto? artist;

    public LoginResponse? Instance
    {
        get => instance;
    }

    public ArtistDto? Artist { get => artist; }

    public bool IsAuthenticated { get => instance is not null; }
    public bool IsArtist { get => IsAuthenticated && artist is not null; }

    public LoginManager(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<LoginResponse?> Login(LoginCredentials loginCredentials)
    {
        using StringContent content = new StringContent(
            JsonSerializer.Serialize(loginCredentials),
            Encoding.UTF8,
            "application/json"
        );

        var response = await httpClient.PostAsync("http://localhost:5013/api/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            instance = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (instance is not null && instance.ProfileType == ProfileTypes.ARTIST)
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", instance.JwtToken);
                response = await httpClient.GetAsync("http://localhost:5013/api/Artist/self");

                if (response.IsSuccessStatusCode)
                {
                    artist = await response.Content.ReadFromJsonAsync<ArtistDto>();
                }
            }
        }

        return instance;
    }

    public async Task Logout()
    {
        var response = await httpClient.GetAsync("localhost:5013/api/auth/logout");

        if (response.IsSuccessStatusCode)
        {
            instance = null;
        }
    }
}
