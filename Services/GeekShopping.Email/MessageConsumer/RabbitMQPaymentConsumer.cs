using GeekShopping.Email.Messages;
using GeekShopping.Email.Repository;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Text.Json;

namespace GeekShopping.Email.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly EmailRepository _repository;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string EXCHANGE_NAME = "DirectPaymentUpdateExchange";
    private const string PAYMENT_EMAIL_UPDATE_QUEUE = "PaymentEmailUpdateQueueName";

    public RabbitMQPaymentConsumer(EmailRepository repository)
    {
        _repository = repository;
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct);
        _channel.QueueDeclare(PAYMENT_EMAIL_UPDATE_QUEUE, false, false, false, null);
        _channel.QueueBind(PAYMENT_EMAIL_UPDATE_QUEUE, EXCHANGE_NAME, "PaymentEmail");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var message = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);
            ProcessLog(message).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };

        _channel.BasicConsume(PAYMENT_EMAIL_UPDATE_QUEUE, false, consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessLog(UpdatePaymentResultMessage message)
    {
        try
        {
            await _repository.LogEmail(message);
        }
        catch (Exception)
        {
            //log
            throw;
        }
    }
}
