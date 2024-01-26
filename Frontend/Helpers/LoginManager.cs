using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using MyMusic.ViewModels;

namespace MyMusic.Frontend.Helpers;

public static class LoginManager
{
    private static LoginResponse? instance = null;

    public static LoginResponse? Instance
    {
        get => instance;
    }

    public static async Task<LoginResponse?> Login(LoginCredentials loginCredentials, HttpClient httpClient)
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
        }

        return instance;
    }

    public static async Task Logout(HttpClient httpClient)
    {
        var response = await httpClient.GetAsync("localhost:5013/api/auth/logout");

        if (response.IsSuccessStatusCode)
        {
            instance = null;
        }
    }
}
