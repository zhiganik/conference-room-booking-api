using ConferenceRoomBooking.TelegramNotifier.Queueing;
using ConferenceRoomBooking.TelegramNotifier.Web.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.TelegramNotifier.Web.Controllers;

/// <summary>Queues text messages to be broadcast to every Telegram chat that has messaged the
/// bot. Internal-only — see docker-compose.yml, this service is not published on a host port.</summary>
[ApiController]
[Route("api/[controller]")]
public class TelegramController(IAlertMessageQueue alertMessageQueue) : ControllerBase
{
    /// <summary>Queues <paramref name="request"/>'s text for delivery to every subscribed chat
    /// (i.e. every chat that has ever sent the bot a message — see the README for how chats
    /// subscribe). Delivery happens asynchronously in the background, so this returns before any
    /// message actually reaches Telegram; a chat that has blocked the bot is dropped from the
    /// subscriber list the next time a broadcast reaches it.</summary>
    /// <param name="request">The message text to deliver.</param>
    /// <response code="202">The message was queued for delivery.</response>
    /// <response code="400">The message failed validation (empty or too long).</response>
    [HttpPost("push")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PushMessage([FromBody] PushMessageRequest request, CancellationToken cancellationToken)
    {
        await alertMessageQueue.EnqueueAsync(request.Message, cancellationToken);
        return Accepted();
    }
}
