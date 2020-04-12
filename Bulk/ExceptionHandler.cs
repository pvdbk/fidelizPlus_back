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
        private LogService LogService { get; }

        public ExceptionHandler(RequestDelegate next, LogService logService)
        {
            Next = next;
            LogService = logService;
        }

        public async Task Invoke(HttpContext context)
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
                        LogService.AddError(jsonToSend);
                        jsonToSend = "\"Server error\"";
                    }
                    context.Response.ContentType = "application/json";
                    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(jsonToSend));
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
