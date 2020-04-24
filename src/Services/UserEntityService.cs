using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using Repositories;
    using Identification;

    public class UserEntityService<TEntity, TPrivate, TPublic, TAccount, TAccountDTO> : CrudService<TEntity, TPrivate>
        where TEntity : UserEntity<TAccount>, new()
        where TPrivate : UserDTO<TAccountDTO>, new()
        where TPublic : class, new()
        where TAccount : Account, new()
        where TAccountDTO : AccountDTO, new()
    {
        public CrudService<User, TPrivate> UserService { get; }
        public CrudService<TAccount, TAccountDTO> AccountService { get; }
        public CommercialLinkService ClService { get; }
        public RelatedToBothService<Purchase, PurchaseDTO> PurchaseService { get; }
        public Action<TEntity> SeekReferences { get; }
        public Credentials Credentials { get; }

        public UserEntityService(
            UserEntityRepository<TEntity, TAccount> repo,
            CrudService<User, TPrivate> userService,
            AccountService<TAccount, TAccountDTO> accountService,
            CommercialLinkService clService,
            RelatedToBothService<Purchase, PurchaseDTO> purchaseService,
            Credentials credentials
        ) : base(repo)
        {
            UserService = userService;
            AccountService = accountService;
            ClService = clService;
            PurchaseService = purchaseService;
            Credentials = credentials;
            SeekReferences = repo.SeekReferences;
            UnexpectedForSaving = new string[] { "Id", "CreationTime" };
            UnexpectedForUpdating = new string[] { "Id", "CreationTime" };
        }

        public void CheckCredentials(int id)
        {
            if (
                !Credentials.AreSetted ||
                Credentials.EntityType != typeof(TEntity) ||
                Credentials.Id != id
            )
            {
                throw new AppException("Unauthorized operation", 400);
            }
        }

        public TAccount DTOToAccount(TAccountDTO dto) => AccountService.DTOToEntity(dto);

        public TAccountDTO AccountToDTO(TAccount account) => AccountService.EntityToDTO(account);

        public TEntity DTOToUserEntity(TPrivate dto, int userId, int accountId)
        {
            TEntity ret = dto.CastAs<TEntity>();
            ret.UserId = userId;
            ret.AccountId = accountId;
            return ret;
        }

        public override TPrivate EntityToDTO(TEntity entity)
        {
            SeekReferences(entity);
            TPrivate ret = base.EntityToDTO(entity);
            TPrivate toMergeWithRet = UserService.EntityToDTO(entity.User);
            IEnumerable<PropertyInfo> props = typeof(TPrivate).GetProps();
            props.ForEach(prop =>
            {
                object value = prop.GetValue(toMergeWithRet);
                if (value != null)
                {
                    prop.SetValue(ret, value);
                }
            });
            ret.Id = entity.Id;
            ret.CreationTime = entity.User.CreationTime;
            ret.Account = AccountService.EntityToDTO(entity.Account);
            return ret;
        }

        public TAccount GetAccount(int? id) => FindEntity(id).Account;

        public void CollectCl(TEntity entity)
        {
            Repo.Entry(entity).Collection("CommercialLink").Load();
            entity.CommercialLink.ForEach(cl => ClService.SeekReferences(cl));
        }


        public virtual IEnumerable<TPublic> FilterOrFindAll(string filter = null) =>
            Entities
                .ToList()
                .Select(entity => EntityToDTO(entity).CastAs<TPublic>())
                .Where(filter.ToTest<TPublic>());

        public object EntityToAuthorizedDTO(TEntity entity)
        {
            object ret = EntityToDTO(entity);
            try
            {
                CheckCredentials(entity.Id);
            }
            catch
            {
                ret = ret.CastAs<TPublic>();
            }
            return ret;
        }

        public TEntity FindEntityByConnectionId(string connectionId)
        {
            TEntity entity = Entities.Where(e => e.ConnectionId == connectionId).FirstOrDefault();
            return entity != null
                ? entity
                : new AppException($"{typeof(TEntity).Name} not found", 404).Throw<TEntity>();
        }

        public object FindByConnectionId(string connectionId) => EntityToAuthorizedDTO(FindEntityByConnectionId(connectionId));

        public object FindById(int id) => EntityToAuthorizedDTO(FindEntity(id));

        public (TPrivate, int) CheckSave(TPrivate dto)
        {
            CheckDTOForSaving(dto);
            TAccountDTO accountDTO = dto.Account;
            AccountService.CheckDTOForSaving(accountDTO);
            if (Entities.Any(entity => entity.ConnectionId == dto.ConnectionId))
            {
                throw new AppException($"'{dto.ConnectionId}' is already used as connectionId", 400);
            }
            User user = UserService.QuickSave(dto);
            Account account = AccountService.QuickSave(accountDTO);
            TEntity entity = DTOToUserEntity(dto, user.Id, account.Id);
            Repo.Save(entity);
            return (EntityToDTO(entity), entity.Id);
        }

        public override TPrivate CheckUpdate(int id, TPrivate dto, HttpContext ctxt)
        {
            CheckCredentials(id);
            CheckDTOForUpdating(dto);
            AccountService.CheckDTOForUpdating(dto.Account);
            if (Entities.Any(e => e.ConnectionId == dto.ConnectionId && e.Id != id))
            {
                throw new AppException($"'{dto.ConnectionId}' is already used as connectionId", 400);
            }
            TEntity entity = FindEntity(id);
            IRequestCookieCollection cookies = ctxt.Request.Cookies;
            if (
                entity.User.Password != dto.Password &&
                cookies.ContainsKey(Credentials.PASSWORD_KEY) &&
                !String.IsNullOrEmpty(cookies[Credentials.PASSWORD_KEY])
            )
            {
                ctxt.Response.Cookies.Append(Credentials.PASSWORD_KEY, dto.Password);
            }
            dto.Account.Balance = entity.Account.Balance;
            int userId = entity.UserId;
            entity = DTOToUserEntity(dto, userId, id);
            entity.Id = id;
            entity = Repo.Update(entity);
            entity.User = UserService.QuickUpdate(userId, dto);
            entity.Account = AccountService.QuickUpdate(entity.AccountId, dto.Account);
            return EntityToDTO(entity);
        }

        public TAccountDTO GetAccountDTO(int id)
        {
            CheckCredentials(id);
            return AccountToDTO(GetAccount(id));
        }

        public IEnumerable<PurchaseDTO> Purchases(int id, string filter)
        {
            CheckCredentials(id);
            Func<PurchaseDTO, bool> test = filter.ToTest<PurchaseDTO>();
            TEntity entity = FindEntity(id);
            CollectCl(entity);
            entity.CommercialLink.ForEach(cl => ClService.CollectPurchases(cl));
            return entity.CommercialLink.Reduce<CommercialLink, IEnumerable<PurchaseDTO>>(
                (ret, cl) => ret.Concat(
                    cl.Purchase
                        .Select(purchase => PurchaseService.EntityToDTO(purchase))
                        .Where(test)
                ),
                Enumerable.Empty<PurchaseDTO>()
            );
        }
    }
}
