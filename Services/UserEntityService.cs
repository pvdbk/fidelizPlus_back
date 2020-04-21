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
        public RelatedToBothService<Purchase, PurchaseDTO> PurchaseService { get; }
        public Action<TEntity> SeekReferences { get; }

        public UserEntityService(
            UserEntityRepository<TEntity, TAccount> repo,
            CrudService<User, TDTO> userService,
            AccountService<TAccount, TAccountDTO> accountService,
            CommercialLinkService clService,
            RelatedToBothService<Purchase, PurchaseDTO> purchaseService
        ) : base(repo)
        {
            UserService = userService;
            AccountService = accountService;
            ClService = clService;
            PurchaseService = purchaseService;
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
            TEntity ret = dto.CastAs<TEntity>();
            ret.UserId = userId;
            ret.AccountId = accountId;
            return ret;
        }

        public override TDTO EntityToDTO(TEntity entity)
        {
            if (entity.User == null)
            {
                SeekReferences(entity);
            }
            TDTO ret = base.EntityToDTO(entity);
            TDTO toMergeWithRet = UserService.EntityToDTO(entity.User);
            IEnumerable<PropertyInfo> props = typeof(TDTO).GetProps();
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

        public override (TDTO, int) CheckSave(TDTO dto)
        {
            CheckDTOForSaving(dto);
            TAccountDTO accountDTO = dto.Account;
            AccountService.CheckDTOForSaving(accountDTO);
            if (Repo.FindAll().Any(entity => entity.ConnectionId == dto.ConnectionId))
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
            if (Repo.FindAll().Any(e => e.ConnectionId == dto.ConnectionId && e.Id != id))
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

        public IEnumerable<PurchaseDTO> Purchases(int id, string filter)
        {
            Func<PurchaseDTO, bool> test = filter.ToTest<PurchaseDTO>();
            TEntity entity = FindEntity(id);
            CollectCl(entity);
            IEnumerable<CommercialLink> cls = entity.CommercialLink;
            var ret = new List<PurchaseDTO>();
            foreach (CommercialLink cl in cls)
            {
                ClService.CollectPurchases(cl);
                IEnumerable<PurchaseDTO> toAdd = cl.Purchase
                    .Select(purchase => PurchaseService.EntityToDTO(purchase))
                    .Where(test);
                ret = ret.Concat(toAdd).ToList();
            }
            return ret;
        }
    }
}
