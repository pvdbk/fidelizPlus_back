using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace fidelizPlus_back.Identification
{
	public class SessionHandler
	{
		public const string CREDENTIALS_KEY = "credentials";
		private RequestDelegate Next { get; }

		public SessionHandler(RequestDelegate next)
		{
			Next = next;
		}

		public async Task Invoke(HttpContext context, CredentialsHandler credentialsHandler)
		{
			string credentialsString = context.Session.GetString(CREDENTIALS_KEY);
			if (String.IsNullOrEmpty(credentialsString))
			{
				try
				{
					IRequestCookieCollection cookies = context.Request.Cookies;
					credentialsHandler.SetAndCheck(cookies);
					if (credentialsHandler.AreSetted)
					{
						context.Session.SetString(CREDENTIALS_KEY, credentialsHandler.GetString);
					}
				}
				catch (Break)
				{
					throw new Break("Bad cookies", BreakCode.ErrCredentials, 400);
				}
			}
			else
			{
				credentialsHandler.Set(credentialsString);
			}
			await Next(context);
		}
	}
}
