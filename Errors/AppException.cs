using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fidelizPlus_back.Errors
{
    public class AppException : Exception
    {
        private LogContext logContext;
        public int Status { get; }
        public object Content { get; }

        public AppException(object content, int status, LogContext logContext) : base()
        {
            this.logContext = logContext;
            this.Content = content;
            this.Status = status;
        }

        public async Task HandleFrom(HttpContext context)
        {
            string jsonToSend = JsonSerializer.Serialize(this.Content);
            if (this.Status == 500)
            {
                this.logContext.ErrorLog.Add(new ErrorLog() { Content = jsonToSend });
                this.logContext.SaveChanges();
                jsonToSend = "\"Server error\"";
            }
            context.Response.StatusCode = this.Status;
            context.Response.ContentType = "application/json";
            await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(jsonToSend));
            return;
        }
    }
}
