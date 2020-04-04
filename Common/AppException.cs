using Microsoft.AspNetCore.Mvc;
using System;

namespace fidelizPlus_back
{
    public class AppException : Exception
    {
        public int Status { get; }
        public object Content { get; }

        public AppException(object content, int status = 500) : base()
        {
            this.Content = content is string ? Utils.Quote((string)content) : content;
            this.Status = status;
        }

        public IActionResult HandleFrom(ControllerBase controller)
        {
            IActionResult ret;
            if (this.Status == 500)
            {
                Console.WriteLine(this.Content.ToString());    // To replace by something better
                ret = controller.StatusCode(500, Utils.Quote("Server error"));
            }
            else
            {
                ret =
                    (this.Status == 400) ? controller.BadRequest(this.Content) :
                    (this.Status == 404) ? controller.NotFound(this.Content) :
                    controller.StatusCode(this.Status, this.Content);
            }
            return ret;
        }

        public static T Cast<T>(object content, int status = 500)
        {
            throw new AppException(content, status);
        }
    }
}
