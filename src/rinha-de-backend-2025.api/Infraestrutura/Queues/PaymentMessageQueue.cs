using System.Threading.Channels;
using rinha_de_backend_2025.api.Request;

namespace rinha_de_backend_2025.api.Infraestrutura.Queues;

internal sealed class PaymentMessageQueue : IPaymentMessageQueue
{
    private readonly ChannelWriter<PaymentRequest> _writer;
    private readonly ChannelReader<PaymentRequest> _reader;
    
    public PaymentMessageQueue()
    {
        var options = new UnboundedChannelOptions
        {
            SingleReader = false, // Allow multiple background workers
            SingleWriter = false, // Allow multiple API instances to enqueue
            AllowSynchronousContinuations = false // Better performance for async operations
        };

        var channel = Channel.CreateUnbounded<PaymentRequest>(options);
        _writer = channel.Writer;
        _reader = channel.Reader;
    }
    
    public async Task PublishAsync(PaymentRequest message, CancellationToken cancellationToken = default)
    {
        await _writer.WriteAsync(message, cancellationToken);
    }

    public IAsyncEnumerable<PaymentRequest> ConsumeAsync(CancellationToken cancellationToken = default)
    {
        return _reader.ReadAllAsync(cancellationToken);
    }
}