
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Repository;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly OrderRepository _repository;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string EXCHANGE_NAME = "DirectPaymentUpdateExchange";
    private const string PAYMENT_ORDER_UPDATE_QUEUE = "PaymentOrderUpdateQueueName";

    public RabbitMQPaymentConsumer(OrderRepository repository)
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
        _channel.QueueDeclare(PAYMENT_ORDER_UPDATE_QUEUE, false, false, false, null);
        _channel.QueueBind(PAYMENT_ORDER_UPDATE_QUEUE, EXCHANGE_NAME, "PaymentOrder");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var vo = JsonSerializer.Deserialize<UpdatePaymentResultVO>(content);
            UpdatePaymentStatus(vo).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };

        _channel.BasicConsume(PAYMENT_ORDER_UPDATE_QUEUE, false, consumer);

        return Task.CompletedTask;
    }

    private async Task UpdatePaymentStatus(UpdatePaymentResultVO vo)
    {
        try
        {
            await _repository.UpdateOrderPaymentStatus(vo.OrderId, vo.Status);
        }
        catch (Exception)
        {
            //log
            throw;
        }
    }
}
