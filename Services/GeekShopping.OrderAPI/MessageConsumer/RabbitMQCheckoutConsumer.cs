
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.RabbitMQSender;
using GeekShopping.OrderAPI.Repository;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer;

public class RabbitMQCheckoutConsumer : BackgroundService
{
    private readonly OrderRepository _repository;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IRabbitMQMessageSender _rabbitMQMessageSender;

    public RabbitMQCheckoutConsumer(OrderRepository repository, IRabbitMQMessageSender rabbitMQMessageSender)
    {
        _repository = repository;
        _rabbitMQMessageSender = rabbitMQMessageSender;
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "checkoutqueue", false, false, false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            CheckoutHeaderVO vo = JsonSerializer.Deserialize<CheckoutHeaderVO>(content);
            ProcessOrder(vo).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };

        _channel.BasicConsume("checkoutqueue", false, consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessOrder(CheckoutHeaderVO vo)
    {
        var order = new OrderHeader
        {
            UserId = vo.UserId,
            FirstName = vo.FirstName,
            LastName = vo.LastName,
            OrderDetails = new List<OrderDetail>(),
            CardNumber = vo.CardNumber,
            CouponCode = vo.CouponCode,
            CVV = vo.CVV,
            DiscountAmount = vo.DiscountAmount,
            Email = vo.Email,
            ExpiryMonthYear = vo.ExpiryMonthYear,
            PurchaseAmount = vo.PurchaseAmount,
            OrderTime = DateTime.Now,
            PaymentStatus = false,
            Phone = vo.Phone,
            DateTime = vo.DateTime
        };

        foreach (var detail in vo.CartDetails)
        {
            var orderDetail = new OrderDetail
            {
                ProductId = detail.ProductId,
                ProductName = detail.Product.Name,
                Price = detail.Product.Price,
                Count = detail.Count
            };

            order.CartTotalItens += detail.Count;
            order.OrderDetails.Add(orderDetail);
        }

        await _repository.AddOrder(order);

        var payment = new PaymentVO
        {
            Name = order.FirstName + " " + order.LastName,
            CardNuber = order.CardNumber,
            CVV = order.CVV,
            ExpiryMonthYear = order.ExpiryMonthYear,
            OrderId = order.Id,
            PurchaseAmount = order.PurchaseAmount,
            Email = order.Email
        };

        try
        {
            _rabbitMQMessageSender.SendMessage(payment, "orderpaymentprocessqueue");
        }
        catch (Exception)
        {
            //log
            throw;
        }
    }
}
