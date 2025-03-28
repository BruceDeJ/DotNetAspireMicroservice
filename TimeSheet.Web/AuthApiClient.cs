using System.Net.Http;
using System.Net.Http.Headers;

namespace TimeSheet.Web;

public class AuthApiClient(HttpClient httpClient)
{
    public async Task<bool> Register(string email, string password, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/Register", new
        {
            email,
            password
        });

        return response.IsSuccessStatusCode;
    }

    public async Task<(string, bool)> Login(string email, string password, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/Login", new
        {
            Email = email,
            Password = password
        });

        string authGuid = await response.Content.ReadFromJsonAsync<string>() ?? string.Empty;

        return (authGuid, response.IsSuccessStatusCode);
    }

    public async Task<bool> LogOut(string loginGuid, CancellationToken cancellationToken = default)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginGuid);

        var response = await httpClient.GetAsync("/Logout");

        return response.IsSuccessStatusCode ? true : false;
    }
}