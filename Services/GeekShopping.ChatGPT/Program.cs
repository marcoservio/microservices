using GeekShopping.ChatGPT.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.AddChatGpt();
builder.AddSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger();

var app = builder.Build();

app.UseSwaggerDoc();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
