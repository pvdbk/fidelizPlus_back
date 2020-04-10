using System;

namespace fidelizPlus_back
{
    using AppDomain;

    public class AccountManager<T> where T : Account
    {
        private T Account { get; }
        private BankManager BankManager { get; }
        private string AccountString { get; }

        public AccountManager(T account, BankManager bankManager)
        {
            Account = account;
            BankManager = bankManager;
            dynamic acnt = account;
            AccountString = account is ClientAccount ? acnt.ExternalAccount : acnt.Gni;
        }

        public bool Add(decimal amount)
        {
            if (amount == 0)
            {
                Console.WriteLine($"Nothing has been done in {AccountString}");
            }
            else if (amount > 0)
            {
                Console.WriteLine($"{amount} has been added in {AccountString}");
            }
            else
            {
                Console.WriteLine($"{amount} has been taken from {AccountString}");
            }
            return true;
        }
    }
}
