using GeekShopping.MessageBus;
using GeekShopping.PaymentAPI.Messages;

using Microsoft.AspNetCore.Components;

using RabbitMQ.Client;

using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.RabbitMQSender;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
    private readonly string _hostName;
    private readonly string _password;
    private readonly string _userName;
    private IConnection _connection;
    private const string EXCHANGE_NAME = "DirectPaymentUpdateExchange";
    private const string PAYMENT_EMAIL_UPDATE_QUEUE = "PaymentEmailUpdateQueueName";
    private const string PAYMENT_ORDER_UPDATE_QUEUE = "PaymentOrderUpdateQueueName";

    public RabbitMQMessageSender()
    {
        _hostName = "localhost";
        _password = "guest";
        _userName = "guest";
    }

    public void SendMessage(BaseMessage message)
    {
        if (ConnectionExists())
        {
            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct, durable: false);

            channel.QueueDeclare(PAYMENT_EMAIL_UPDATE_QUEUE, false, false, false, null);
            channel.QueueDeclare(PAYMENT_ORDER_UPDATE_QUEUE, false, false, false, null);

            channel.QueueBind(PAYMENT_EMAIL_UPDATE_QUEUE, EXCHANGE_NAME, "PaymentEmail");
            channel.QueueBind(PAYMENT_ORDER_UPDATE_QUEUE, EXCHANGE_NAME, "PaymentOrder");

            byte[] body = GetMessageAsByteArray(message);

            channel.BasicPublish(exchange: EXCHANGE_NAME, "PaymentEmail", basicProperties: null, body: body);
            channel.BasicPublish(exchange: EXCHANGE_NAME, "PaymentOrder", basicProperties: null, body: body);
        }
    }

    private static byte[] GetMessageAsByteArray(BaseMessage message)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize((UpdatePaymentResultMessage)message, options);

        return Encoding.UTF8.GetBytes(json);
    }

    private bool ConnectionExists()
    {
        if (_connection != null)
            return true;

        CreateConnection();

        return _connection != null;
    }

    private void CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            _connection = factory.CreateConnection();
        }
        catch (Exception)
        {
            //Log exception
            throw;
        }
    }
}
