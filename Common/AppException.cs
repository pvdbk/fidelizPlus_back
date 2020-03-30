using Microsoft.AspNetCore.Mvc;
using System;

namespace fidelizPlus_back
{
    public class AppException : Exception
    {
        public int Status { get; }

        public AppException(string message, int status = 500) : base(message)
        {
            this.Status = status;
        }

        public IActionResult HandleFrom(ControllerBase controller)
        {
            IActionResult ret;
            if (this.Status == 500)
            {
                Console.WriteLine(this.Message);    // To replace by something better
                ret = controller.StatusCode(500, Utils.Quote("Server error"));
            }
            else
            {
                ret =
                    (this.Status == 400) ? controller.BadRequest(Utils.Quote(this.Message)) :
                    (this.Status == 404) ? controller.NotFound(Utils.Quote(this.Message)) :
                    controller.StatusCode(this.Status, Utils.Quote(this.Message));
            }
            return ret;
        }

        public static T Cast<T>(string message, int status = 500)
        {
            throw new AppException(message, status);
        }
    }
}
