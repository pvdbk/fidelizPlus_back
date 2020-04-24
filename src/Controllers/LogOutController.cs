using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace fidelizPlus_back.Controllers
{
    using Identification;

    [Route("[controller]")]
    [ApiController]
    public class LogOutController : ControllerBase
    {
        private const string USER_TYPE_KEY = Credentials.USER_TYPE_KEY;
        private const string ID_KEY = Credentials.ID_KEY;
        private const string PASSWORD_KEY = Credentials.PASSWORD_KEY;
        private Credentials Credentials { get; }

        public LogOutController(Credentials credentials)
        {
            Credentials = credentials;
        }

        [HttpPost]
        [Route("")]
        public IActionResult Disconnect()
        {
            ISession session = HttpContext.Session;
            if (String.IsNullOrEmpty(session.GetString(SessionHandler.CREDENTIALS_KEY)))
            {
                throw new AppException("No connection is enabled", 400);
            }
            session.Remove(SessionHandler.CREDENTIALS_KEY);
            var cookieOptions = new CookieOptions()
            {
                Expires = new DateTimeOffset(new DateTime(0), TimeSpan.Zero)
            };
            Action<string> delCookie = key =>
                HttpContext.Response.Cookies.Append(key, "", cookieOptions);
            delCookie(USER_TYPE_KEY);
            delCookie(ID_KEY);
            delCookie(PASSWORD_KEY);
            return NoContent();
        }
    }
}
