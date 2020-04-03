using System;
using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Services
{
    using Models;
    using DTO;
    using Repositories;

    public class AccountsStandardService : Service<AccountDTO>
    {
        private AccountRepository repo;
        private static readonly Func<string, bool> IsRequiredProp = prop => prop != "Id";

        public AccountsStandardService(AccountRepository repo)
        {
            this.repo = repo;
        }

        public AccountDTO ToDTO(Account account)
        {
            return new AccountDTO()
            {
                Id = account.Id,
                ClientId = account.ClientId,
                Balance = account.Balance
            };
        }

        public Account DTOToAccount(int? id, AccountDTO dto)
        {
            Account account = new Account()
            {
                ClientId = dto.ClientId,
                Balance = dto.Balance
            };
            if (id != null)
            {
                account.Id = (int)id;
            }
            return account;
        }

        public Account Find(int id)
        {
            Account account = this.repo.FindById(id);
            if (account == default(Account))
            {
                throw new AppException("Account not found", 404);
            }
            return account;
        }

        public void Delete(int id)
        {
            this.repo.Delete(id);
        }

        public IEnumerable<AccountDTO> Filter(string filter)
        {
            return this.repo.Filter(new Tree(filter)).Select(this.ToDTO);
        }

        public IEnumerable<AccountDTO> FindAll()
        {
            IEnumerable<Account> accounts = this.repo.FindAll().ToList();
            return accounts.Select(this.ToDTO);
        }

        public AccountDTO FindById(int id)
        {
            return this.ToDTO(this.Find(id));
        }

        public AccountDTO Save(AccountDTO dto)
        {
            Utils.CheckDTO(dto, prop => prop != "Id");
            Account account = DTOToAccount(null, dto);
            this.repo.Save(account);
            return this.ToDTO(account);
        }

        public AccountDTO Update(int id, AccountDTO dto)
        {
            Account account = this.Find(id);
            Utils.CheckDTO(dto, prop => prop != "Id");
            account = this.repo.Update(this.DTOToAccount(id, dto));
            return this.ToDTO(account);
        }
    }
}
