using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace fidelizPlus_back.Controllers
{
	using Identification;

	[Route("[controller]")]
	[ApiController]
	public class LogInController : ControllerBase
	{
		private const string USER_TYPE_KEY = Credentials.USER_TYPE_KEY;
		private const string ID_KEY = Credentials.ID_KEY;
		private const string PASSWORD_KEY = Credentials.PASSWORD_KEY;
		private CredentialsHandler CredentialsHandler { get; }

		public LogInController(CredentialsHandler credentialsHandler)
		{
			CredentialsHandler = credentialsHandler;
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
				throw new Break(
					"A connection is still enabled",
					BreakCode.ErrSession,
					400
				);
			}
			object ret = CredentialsHandler.SetAndCheck(dto);
			if (stayConnected)
			{
				var cookieOptions = new CookieOptions() { IsEssential = true };
				Action<string, string> addCookie = (key, value) =>
					HttpContext.Response.Cookies.Append(key, value, cookieOptions);
				addCookie(USER_TYPE_KEY, $"{CredentialsHandler.GetUserType}");
				addCookie(ID_KEY, $"{CredentialsHandler.GetId}");
				addCookie(PASSWORD_KEY, $"{CredentialsHandler.GetPassword}");
			}
			session.SetString(SessionHandler.CREDENTIALS_KEY, CredentialsHandler.GetString);
			return Ok(ret);
		}
	}
}
