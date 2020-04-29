using System;

namespace fidelizPlus_back.Identification
{
    using AppDomain;

    public class Credentials
    {
        public const string USER_TYPE_KEY = "userType";
        public const string ID_KEY = "id";
        public const string PASSWORD_KEY = "password";
        public string ClientUserType => typeof(Client).Name.FirstToLower();
        public string TraderUserType => typeof(Trader).Name.FirstToLower();
        public string UserType { get; private set; }
        public int Id { get; private set; }
        public bool AreSetted { get; private set; }

        public Credentials() => AreSetted = false;

        public void Set(string userType, int id)
        {
            if (AreSetted)
            {
                throw new AppException("Credentials are alareday setted");
            }
            Id = id;
            UserType = userType;
            AreSetted = true;
        }

        public Type EntityType => UserType == ClientUserType ? typeof(Client) : typeof(Trader);
    }
}
