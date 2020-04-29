using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace fidelizPlus_back
{
    using LogDomain;

    public class ExceptionHandler
    {
        private RequestDelegate Next { get; }

        public ExceptionHandler(RequestDelegate next) => Next = next;

        private async Task SendHttpResponse(HttpContext context, LogService logService, AppException e)
        {
            HttpResponse response = context.Response;
            byte[] bodyContent;
            if (e is SendPic)
            {
                response.ContentType = Services.LogoService.MIME_TYPE;
                response.StatusCode = 200;
                bodyContent = (byte[])e.Content;
            }
            else
            {
                response.ContentType = "application/json";
                response.StatusCode = e.Status;
                string errorJson = e.Content.ToJson();
                if (e.Status == 500)
                {
                    logService.AddError(errorJson);
                    bodyContent = "Server error".Quote().ToBytes();
                }
                else
                {
                    bodyContent = errorJson.ToBytes();
                }
            }
            await context.Response.Body.WriteAsync(bodyContent);
        }

        public async Task Invoke(HttpContext context, LogService logService)
        {
            try
            {
                await Next(context);
            }
            catch (AppException e)
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    logService.AddError(e.Content.ToJson());
                }
                else
                {
                    await SendHttpResponse(context, logService, e);
                }
            }
        }
    }
}
