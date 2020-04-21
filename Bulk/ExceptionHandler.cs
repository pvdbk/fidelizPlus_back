using Microsoft.AspNetCore.Http;
using System.Text;
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
                string errorJson = e.Content.ToJson();
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = e.Status;
                    if (e.Status == 500)
                    {
                        logService.AddError(errorJson);
                        errorJson = "Server error".Quote();
                    }
                    context.Response.ContentType = "application/json";
                    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(errorJson));
                }
                else
                {
                    logService.AddError(errorJson);
                }
            }
            return;
        }
    }
}
