using ConferenceRoomBooking.Bll.Common.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ConferenceRoomBooking.Functions.Functions;

public class WebhookFunction(IWebhookSubscriptionHandler webhookSubscriptionHandler)
{
    [Function("WebhookFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "webhook")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        await webhookSubscriptionHandler.HandleAsync(req.Body, cancellationToken);
        return new OkResult();
    }
}
