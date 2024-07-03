using Microsoft.AspNetCore.Mvc;

using OpenAI_API;
using OpenAI_API.Chat;

namespace GeekShopping.ChatGPT.Controllers;

[Route("bot/[controller]")]
public class ChatController : Controller
{
    private readonly OpenAIAPI _chatGPT;

    public ChatController(OpenAIAPI chatGPT)
    {
        _chatGPT = chatGPT;
    }

    [HttpGet]
    public async Task<IActionResult> Chat([FromQuery(Name = "prompt")] string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            return BadRequest("O prompt não pode estar vazio.");

        var result = await _chatGPT.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = "gpt-3.5-turbo",
            Messages = [new() { Role = ChatMessageRole.User, TextContent = prompt }]
        });

        var response = result?.Choices[0]?.Message?.TextContent;

        if (string.IsNullOrWhiteSpace(response))
            return BadRequest("Não foi possível obter uma resposta válida.");

        return Ok(response);
    }
}
