using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace fidelizPlus_back.Identification
{
    using AppDomain;
    using Services;

    public class Credentials
    {
        public const string USER_TYPE_KEY = "userType";
        public const string ID_KEY = "id";
        public const string PASSWORD_KEY = "password";
        public UserType UserType { get; private set; }
        public int Id { get; private set; }
        public string Password { get; private set; }
        private bool IsSetted { get; set; }
        private ClientService ClientService { get; }
        private TraderService TraderService { get; }

        public Credentials(ClientService clientService, TraderService traderService)
        {
            ClientService = clientService;
            TraderService = traderService;
            IsSetted = false;
        }

        public override string ToString() => IsSetted
            ?
                "{" +
                $"{USER_TYPE_KEY.Quote()}:{(int)UserType}," +
                $"{ID_KEY.Quote()}:{Id}" +
                "}"
            : null;

        public void Check()
        {
            string password = UserType == UserType.Client
                ? ClientService.FindById(Id).Password
                : TraderService.FindById(Id).Password;
            if (Password != password)
            {
                throw new AppException("Bad password", 400);
            }
        }

        public void Set(IRequestCookieCollection cookies)
        {
            Func<string, bool> IsAvailable = key =>
                cookies.ContainsKey(key) &&
                !String.IsNullOrEmpty(cookies[key]);
            if (new string[] { USER_TYPE_KEY, ID_KEY, PASSWORD_KEY }.All(IsAvailable))
            {
                UserType = (UserType)cookies[USER_TYPE_KEY].ToInt();
                Id = cookies[ID_KEY].ToInt();
                Password = cookies[PASSWORD_KEY];
                Check();
                IsSetted = true;
            }
        }

        public void Set(CredentialsDTO dto)
        {
            Id = dto.Id;
            UserType = dto.UserType;
            Password = dto.Password;
            Check();
            IsSetted = true;
        }

        public void Set(string credentialsJson)
        {
            try
            {
                Tree credentialsTree = new Tree(credentialsJson);
                Id = credentialsTree.Get(ID_KEY).IntValue;
                UserType = (UserType)credentialsTree.Get(USER_TYPE_KEY).IntValue;
            }
            catch
            {
                throw new AppException("Bad or missing credentials");
            }
            IsSetted = true;
        }
    }
}
