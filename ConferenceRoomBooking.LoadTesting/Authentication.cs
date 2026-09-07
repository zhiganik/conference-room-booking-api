using System.Text;
using System.Text.Json;

namespace ConferenceRoomBooking.LoadTesting;

public sealed class Authentication(HttpClient httpClient)
{
    public async Task<string> LoginAsync(string email, string password)
    {
        var payload = JsonSerializer.Serialize(new { Email = email, Password = password });
        using var response = await httpClient.PostAsync("api/auth/login",
            new StringContent(payload, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(body).RootElement.GetProperty("accessToken").GetString()!;
    }
}
