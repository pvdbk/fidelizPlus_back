using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace fidelizPlus_back
{
	using LogDomain;

	public class ExceptionHandler
	{
		private RequestDelegate Next { get; }

		public ExceptionHandler(RequestDelegate next) => Next = next;

		private async Task SendHttpResponse(HttpContext context, int status, string contentType, byte[] content)
		{
			HttpResponse response = context.Response;
			response.StatusCode = status;
			response.ContentType = contentType;
			await context.Response.Body.WriteAsync(content);
		}

		public async Task Invoke(HttpContext context, LogService logService)
		{
			try
			{
				await Next(context);
			}
			catch (Break b)
			{
				string errorJson = new
				{
					content = b.Content,
					errCode = b.Code.ToString()
				}.ToJson();
				if(b.Status == 500)
				{
					logService.AddError(errorJson);
					await SendHttpResponse(context, 500, "text/plain", "Server error".ToBytes());
				}
				else if (!context.WebSockets.IsWebSocketRequest)
				{
					await SendHttpResponse(context, b.Status, "application/json", errorJson.ToBytes());
				}
			}
			catch (SendPic s)
			{
				await SendHttpResponse(
					context,
					200,
					Services.LogoService.MIME_TYPE,
					s.PicBytes
				);
			}
		}
	}
}
