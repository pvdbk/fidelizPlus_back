using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fidelizPlus_back
{
    public class AppException : Exception
    {
        public int Status { get; }
        public object Content { get; }

        public AppException(object content, int status = 500) : base()
        {
            this.Content = content;
            this.Status = status;
        }

        public async Task HandleFrom(HttpContext context)
        {
            string jsonToSend;
            if (this.Status == 500)
            {
                Console.WriteLine(this.Content);    // To replace by something better
                jsonToSend = "\"Server error\"";
            }
            else
            {
                jsonToSend = JsonSerializer.Serialize(this.Content);
            }
            context.Response.StatusCode = this.Status;
            context.Response.ContentType = "application/json";
            await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(jsonToSend));
            return;
        }

        public T Cast<T>()
        {
            throw this;
        }
    }
}
