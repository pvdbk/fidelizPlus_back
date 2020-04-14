using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;
    using Repositories;

    public class UserEntityService<TEntity, TDTO, TAccount, TAccountDTO> : CrudService<TEntity, TDTO>
        where TEntity : UserEntity<TAccount>, new()
        where TDTO : UserDTO<TAccountDTO>, new()
        where TAccount : Account, new()
        where TAccountDTO : AccountDTO, new()
    {
        public CrudService<User, TDTO> UserService { get; }
        public CrudService<TAccount, TAccountDTO> AccountService { get; }
        public CommercialLinkService ClService { get; }
        public Action<TEntity> SeekReferences { get; }

        public UserEntityService(
            UserEntityRepository<TEntity, TAccount> repo,
            Utils utils,
            CrudService<User, TDTO> userService,
            AccountService<TAccount, TAccountDTO> accountService,
            CommercialLinkService clService
        ) : base(repo, utils)
        {
            UserService = userService;
            AccountService = accountService;
            ClService = clService;
            SeekReferences = repo.SeekReferences;
            UnexpectedForSaving = new string[] { "Id", "CreationTime" };
            UnexpectedForUpdating = new string[] { "Id", "CreationTime" };
        }

        public TAccount DTOToAccount(TAccountDTO dto)
        {
            return AccountService.DTOToEntity(dto);
        }

        public TAccountDTO AccountToDTO(TAccount account)
        {
            return AccountService.EntityToDTO(account);
        }

        public TEntity DTOToUserEntity(TDTO dto, int userId, int accountId)
        {
            TEntity entity = Utils.Cast<TEntity, TDTO>(dto);
            entity.UserId = userId;
            entity.AccountId = accountId;
            return entity;
        }

        public override TDTO EntityToDTO(TEntity entity)
        {
            if (entity.User == null)
            {
                SeekReferences(entity);
            }
            TDTO ret = base.EntityToDTO(entity);
            TDTO toMergeWithRet = UserService.EntityToDTO(entity.User);
            IEnumerable<PropertyInfo> props = Utils.GetProps<TDTO>();
            foreach (PropertyInfo prop in props)
            {
                object value = prop.GetValue(toMergeWithRet);
                if (value != null)
                {
                    prop.SetValue(ret, value);
                }
            }
            ret.Id = entity.Id;
            ret.CreationTime = entity.User.CreationTime;
            ret.Account = AccountService.EntityToDTO(entity.Account);
            return ret;
        }

        public Tree ConvertFilter(Tree filterArg)
        {
            Tree ret = null;
            if (filterArg != null)
            {
                Tree dtoTree = Utils.ExtractTree<TDTO>(filterArg);
                Console.WriteLine(dtoTree.Json);
                ret = Utils.ExtractTree<TEntity>(dtoTree);
                Tree userTree = Utils.ExtractTree<User>(dtoTree, "user");
                userTree.Remove("id");
                ret.Add(userTree);
                Tree accountTree = filterArg.Get("account");
                if (accountTree != null)
                {
                    accountTree = Utils.ExtractTree<TAccountDTO>(accountTree, "account");
                    if (accountTree != null)
                    {
                        ret.Add(accountTree);
                    }
                }
            }
            return ret;
        }

        public override IEnumerable<TEntity> GetEntities(Tree filterArg)
        {
            Tree filterTree = ConvertFilter(filterArg);
            Func<TEntity, bool> filterFunc = Utils.TreeToTest<TEntity>(filterTree);
            return Repo.GetEntities(filterFunc);
        }

        private IEnumerable<TEntity> HavingConnectionId(string connectionId)
        {
            Tree filterTree = Utils.ToTree(new { connectionId });
            Func<TEntity, bool> filterFunc = Utils.TreeToTest<TEntity>(filterTree);
            return Repo.GetEntities(filterFunc);
        }

        public override (TDTO, int) CheckSave(TDTO dto)
        {
            CheckDTOForSaving(dto);
            TAccountDTO accountDTO = dto.Account;
            AccountService.CheckDTOForSaving(accountDTO);
            if (HavingConnectionId(dto.ConnectionId).Count() != 0)
            {
                throw new AppException($"'{dto.ConnectionId}' is already used as connectionId", 400);
            }
            User user = UserService.QuickSave(dto);
            Account account = AccountService.QuickSave(accountDTO);
            TEntity entity = DTOToUserEntity(dto, user.Id, account.Id);
            Repo.Save(entity);
            return (EntityToDTO(entity), entity.Id);
        }

        public override TDTO CheckUpdate(int id, TDTO dto)
        {
            CheckDTOForUpdating(dto);
            AccountService.CheckDTOForUpdating(dto.Account);
            TEntity entity = FindEntity(id);
            SeekReferences(entity);
            dto.Account.Balance = entity.Account.Balance;
            int userId = entity.UserId;
            if (HavingConnectionId(dto.ConnectionId).Any(e => e.Id != id))
            {
                throw new AppException($"'{dto.ConnectionId}' is already used as connectionId", 400);
            }
            entity = DTOToUserEntity(dto, userId, id);
            entity.Id = id;
            entity = Repo.Update(entity);
            entity.User = UserService.QuickUpdate(userId, dto);
            entity.Account = AccountService.QuickUpdate(entity.AccountId, dto.Account);
            return EntityToDTO(entity);
        }

        public void CollectCl(TEntity entity)
        {
            Repo.Entry(entity).Collection("CommercialLink").Load();
        }

        public TAccount GetAccount(int? id)
        {
            TEntity entity = FindEntity(id);
            SeekReferences(entity);
            return entity.Account;
        }

        public TAccountDTO GetAccountDTO(int? id)
        {
            return AccountToDTO(GetAccount(id));
        }
    }
}
