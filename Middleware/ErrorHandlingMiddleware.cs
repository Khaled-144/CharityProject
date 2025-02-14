namespace CharityProject.Middleware
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;
        private readonly string _discordWebhookUrl = "https://discord.com/api/webhooks/1339965688885936259/k4Q1FZWPadvQRZMzp6StXnmx2B7LTgpp0N6BEg4dc-Qy2O0vUlWy26dpvzvOaLgXbBMJ";

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
            _httpClient = new HttpClient();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await SendErrorToDiscord(ex, context);
                context.Response.Redirect("/Home/Error");
            }
        }

        private async Task SendErrorToDiscord(Exception ex, HttpContext context)
        {
            try
            {
                string stackTrace = ex.StackTrace?.Length > 1000 ? ex.StackTrace.Substring(0, 1000) + "..." : ex.StackTrace;

                var errorMessage = new
                {
                    content = $@"
🚨 **Error Alert!**
🔗 **Path:** `{context.Request.Path}`
📡 **Method:** `{context.Request.Method}`
🖥 **Host:** `{context.Request.Host}`
❌ **Exception:** `{EscapeMarkdown(ex.Message)}`
📝 **Stack Trace:** ```{EscapeMarkdown(stackTrace)}```
"
                };

                var json = JsonSerializer.Serialize(errorMessage);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_discordWebhookUrl, content);
            }
            catch
            {
                // Fail silently to avoid recursion in case of Discord issues
            }
        }

        private string EscapeMarkdown(string input)
        {
            return string.IsNullOrEmpty(input) ? input : input.Replace("`", "'");
        }
    }


}
