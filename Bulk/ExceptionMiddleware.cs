using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fidelizPlus_back
{
    using LogDomain;

    public class ExceptionMiddleware
    {
        private RequestDelegate Next { get; }
        private LogContext LogContext { get; }

        public ExceptionMiddleware(RequestDelegate next, LogContext logCtxt)
        {
            Next = next;
            LogContext = logCtxt;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);
            }
            catch (AppException e)
            {
                string jsonToSend = JsonSerializer.Serialize(e.Content);
                if (e.Status == 500)
                {
                    LogContext.ErrorLog.Add(new ErrorLog() { Content = jsonToSend });
                    LogContext.SaveChanges();
                    jsonToSend = "\"Server error\"";
                }
                context.Response.StatusCode = e.Status;
                context.Response.ContentType = "application/json";
                await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(jsonToSend));
            }
            return;
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
