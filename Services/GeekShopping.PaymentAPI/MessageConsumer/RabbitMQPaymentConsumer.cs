using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentAPI.RabbitMQSender;
using GeekShopping.PaymentProcessor;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IRabbitMQMessageSender _rabbitMQMessageSender;
    private readonly IProcessPayment _processPayment;

    public RabbitMQPaymentConsumer(IProcessPayment processPayment, IRabbitMQMessageSender rabbitMQMessageSender)
    {
        _processPayment = processPayment;
        _rabbitMQMessageSender = rabbitMQMessageSender;

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "orderpaymentprocessqueue", false, false, false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var vo = JsonSerializer.Deserialize<PaymentMessage>(content);
            ProcessPayment(vo).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };

        _channel.BasicConsume("orderpaymentprocessqueue", false, consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessPayment(PaymentMessage vo)
    {
        var result = await _processPayment.PaymentProcessor();

        var paymentResult = new UpdatePaymentResultMessage()
        {
            Status = result,
            OrderId = vo.OrderId,
            Email = vo.Email
        };

        try
        {
            _rabbitMQMessageSender.SendMessage(paymentResult);
        }
        catch (Exception)
        {
            //Log
            throw;
        }
    }
}
