using System.Net.Http.Json;
using ConferenceRoomBooking.Bll.Common.Notifications;

namespace ConferenceRoomBooking.Web.Notifications;

public class HttpTelegramNotifier(HttpClient httpClient) : INotifier
{
    public async Task PushAsync(string message, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/Telegram/push", new { message }, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
