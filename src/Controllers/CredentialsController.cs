using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace fidelizPlus_back.Controllers
{
	using Identification;

	[Route("[controller]")]
	[ApiController]
	public class CredentialsController : ControllerBase
	{
		private const string USER_TYPE_KEY = Credentials.USER_TYPE_KEY;
		private const string ID_KEY = Credentials.ID_KEY;
		private const string PASSWORD_KEY = Credentials.PASSWORD_KEY;
		private CredentialsHandler CredentialsHandler { get; }

		public CredentialsController(CredentialsHandler credentialsHandler)
		{
			CredentialsHandler = credentialsHandler;
		}

		[HttpGet]
		[Route("")]
		public IActionResult GetCredentials() => Ok(CredentialsHandler.GetPrivateDTO);
	}
}
