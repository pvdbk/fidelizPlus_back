using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace fidelizPlus_back.Controllers
{
    using Identification;
    using Services;

    [Route("[controller]")]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private const string USER_TYPE_KEY = Credentials.USER_TYPE_KEY;
        private const string ID_KEY = Credentials.ID_KEY;
        private const string PASSWORD_KEY = Credentials.PASSWORD_KEY;
        private Credentials Credentials { get; }

        public LogInController(MultiService multiService, Credentials credentials)
        {
            Credentials = credentials;
        }

        [HttpPost]
        [Route("")]
        public IActionResult Connect(
            [FromBody] CredentialsDTO dto,
            [FromQuery] bool stayConnected = false
        )
        {
            ISession session = HttpContext.Session;
            if (!String.IsNullOrEmpty(session.GetString(SessionHandler.CREDENTIALS_KEY)))
            {
                throw new AppException("A connection is still enabled", 400);
            }
            Credentials.Set(dto);
            if (stayConnected)
            {
                var cookieOptions = new CookieOptions() { IsEssential = true };
                Action<string, string> addCookie = (key, value) =>
                    HttpContext.Response.Cookies.Append(key, value, cookieOptions);
                addCookie(USER_TYPE_KEY, $"{(int)Credentials.UserType}");
                addCookie(ID_KEY, $"{Credentials.Id}");
                addCookie(PASSWORD_KEY, $"{Credentials.Password}");
            }
            session.SetString(SessionHandler.CREDENTIALS_KEY, Credentials.ToString());
            return NoContent();
        }
    }
}
