using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace fidelizPlus_back.Identification
{
    using Services;

    public class SessionHandler
    {
        public const string CREDENTIALS_KEY = "credentials";
        private RequestDelegate Next { get; }
        private MultiService MultiService { get; }

        public SessionHandler(RequestDelegate next, MultiService multiService)
        {
            Next = next;
            MultiService = multiService;
        }

        public async Task Invoke(HttpContext context, Credentials credentials)
        {
            string credentialsString = context.Session.GetString(CREDENTIALS_KEY);
            if (String.IsNullOrEmpty(credentialsString))
            {
                try
                {
                    IRequestCookieCollection cookies = context.Request.Cookies;
                    credentials.Set(cookies);
                    credentialsString = credentials.ToString();
                    if (credentialsString != null)
                    {
                        context.Session.SetString(CREDENTIALS_KEY, credentialsString);
                    }
                }
                catch (AppException)
                {
                    throw new AppException("Bad cookies", 400);
                }
            }
            else
            {
                credentials.Set(credentialsString);
            }
            await Next(context);
        }
    }
}
