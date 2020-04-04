using System;

namespace fidelizPlus_back
{
    public class AccountManager
    {
        public string Account { get; }

        public AccountManager(string account)
        {
            this.Account = account;
        }

        public bool Update(int amount)
        {
            if (amount == 0)
            {
                Console.WriteLine($"Nothing has been done in {this.Account}");
            }
            else if (amount > 0)
            {
                Console.WriteLine($"{amount} has been added in {this.Account}");
            }
            else
            {
                Console.WriteLine($"{amount} has been taken from {this.Account}");
            }
            return true;
        }
    }
}
