using System.Threading.Channels;

namespace ConferenceRoomBooking.TelegramNotifier.Queueing;

public class AlertMessageQueue : IAlertMessageQueue
{
    private readonly Channel<string> _channel = Channel.CreateUnbounded<string>();

    public ChannelReader<string> Reader => _channel.Reader;

    public ValueTask EnqueueAsync(string message, CancellationToken cancellationToken) =>
        _channel.Writer.WriteAsync(message, cancellationToken);
}
