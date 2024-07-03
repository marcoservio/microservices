using OpenAI_API;

namespace GeekShopping.ChatGPT.Extensions;

public static class ChatGPTExtensions
{
    public static void AddChatGpt(this WebApplicationBuilder builder)
    {
        //var key = configuration["ChatGPT:Key"];
        var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        var chat = new OpenAIAPI(key);

        builder.Services.AddSingleton(chat);
    }
}
