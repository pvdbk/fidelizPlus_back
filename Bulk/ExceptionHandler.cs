using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fidelizPlus_back
{
    using LogDomain;

    public class ExceptionHandler
    {
        private RequestDelegate Next { get; }

        public ExceptionHandler(RequestDelegate next) => Next = next;

        public async Task Invoke(HttpContext context, LogService logService)
        {
            try
            {
                await Next(context);
            }
            catch (AppException e)
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = e.Status;
                    string jsonToSend = JsonSerializer.Serialize(e.Content);
                    if (e.Status == 500)
                    {
                        logService.AddError(jsonToSend);
                        jsonToSend = "\"Server error\"";
                    }
                    context.Response.ContentType = "application/json";
                    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(jsonToSend));
                }
                else
                {
                    string jsonToSend = JsonSerializer.Serialize(e.Content);
                    logService.AddError(jsonToSend);
                }
            }
            return;
        }
    }

    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandler>();
        }
    }
}
