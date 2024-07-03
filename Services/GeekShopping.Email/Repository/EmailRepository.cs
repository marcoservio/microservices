using GeekShopping.Email.Messages;
using GeekShopping.Email.Model;
using GeekShopping.Email.Model.Context;

using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Repository;

public class EmailRepository : IEmailRepository
{
    private readonly DbContextOptions<MySqlContext> _context;

    public EmailRepository(DbContextOptions<MySqlContext> context)
    {
        _context = context;
    }

    public async Task LogEmail(UpdatePaymentResultMessage message)
    {
        var log = new EmailLog()
        {
            Email = message.Email,
            SentDate = DateTime.UtcNow,
            Log = $"Order - {message.OrderId} has been created successfully!"
        };

        await using var db = new MySqlContext(_context);

        db.EmailLogs.Add(log);

        await db.SaveChangesAsync();
    }
}
