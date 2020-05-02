using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace fidelizPlus_back.Identification
{
	using Services;

	public class CredentialsHandler
	{
		private const string USER_TYPE_KEY = Credentials.USER_TYPE_KEY;
		private const string ID_KEY = Credentials.ID_KEY;
		private const string PASSWORD_KEY = Credentials.PASSWORD_KEY;
		private string Password { get; set; }
		private ClientService ClientService { get; }
		private TraderService TraderService { get; }
		private Credentials Credentials { get; }

		public CredentialsHandler(
			Credentials credentials,
			ClientService clientService,
			TraderService traderService
		)
		{
			Password = null;
			Credentials = credentials;
			ClientService = clientService;
			TraderService = traderService;
		}

		public string GetUserType => Credentials.UserType;

		public int GetId => Credentials.Id;

		public bool AreSetted => Credentials.AreSetted;

		public string GetString =>
			"{" +
			$"{USER_TYPE_KEY.Quote()}:{GetUserType.Quote()}," +
			$"{ID_KEY.Quote()}:{GetId}" +
			"}";

		public void Set(string credentialsJson)
		{
			int id;
			string userType;
			Tree credentialsTree = new Tree(credentialsJson);
			id = credentialsTree.Get(ID_KEY).IntValue;
			userType = credentialsTree.Get(USER_TYPE_KEY).StringValue;
			Credentials.Set(userType, id);
		}

		public void SetAndCheck(IRequestCookieCollection cookies)
		{
			Func<string, bool> IsAvailable = key =>
				cookies.ContainsKey(key) &&
				!String.IsNullOrEmpty(cookies[key]);
			if (new string[] { USER_TYPE_KEY, ID_KEY, PASSWORD_KEY }.All(IsAvailable))
			{
				string userType = cookies[USER_TYPE_KEY];
				int id = cookies[ID_KEY].ToInt();
				Password = cookies[PASSWORD_KEY];
				if (Password != Service(userType).FindEntity(id).User.Password)
				{
					throw new Break();
				}
				Credentials.Set(userType, id);
			}
		}

		private dynamic Service(string userType) =>
			userType == Credentials.ClientUserType ? ClientService :
			userType == Credentials.TraderUserType ? TraderService :
			new Break(
				$"Bad {USER_TYPE_KEY}: {userType}",
				BreakCode.ErrCredentials,
				400
			).Throw<object>();

		public object SetAndCheck(CredentialsDTO dto)
		{
			string userType = dto.UserType;
			dynamic entity = Service(userType).FindEntityByConnectionId(dto.ConnectionId);
			if (entity.User.Password != dto.Password)
			{
				throw new Break("Bad password", BreakCode.ErrCredentials, 400);
			}
			Credentials.Set(userType, entity.Id);
			Password = dto.Password;
			return Service(userType).EntityToDTO(entity);
		}

		public string GetPassword =>
			Password != null ? Password :
			AreSetted ? Service(GetUserType).FindEntity(GetId).User.Password :
			null;

		public object GetPrivateDTO => AreSetted ? Service(GetUserType).FindById(GetId) : null;
	}
}
