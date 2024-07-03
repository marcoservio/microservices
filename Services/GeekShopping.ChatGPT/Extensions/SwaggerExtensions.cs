using Microsoft.OpenApi.Models;

namespace GeekShopping.ChatGPT.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ChatGPT ASP.Net 8 Integration",
                Version = "v1",
            });
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        });
    }

    public static void UseSwaggerDoc(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatGPT ASP.Net 8 Integration");
            c.RoutePrefix = "swagger";
        });
    }
}
