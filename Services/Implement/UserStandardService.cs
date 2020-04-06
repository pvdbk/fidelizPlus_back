using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back.Services
{
    using DTO;
    using Models;
    using Repositories;

    public class UserStandardService<TEntity, TDTO> : CrudStandardService<TEntity, TDTO> where TEntity : Entity, UserEntity, new() where TDTO : UserDTO, new()
    {
        public CrudRepository<User> UserRepo { get; }
        public CommercialLinkRepository ClRepo { get; }

        public UserStandardService(
            UserEntityRepository<TEntity> repo,
            Utils utils,
            CrudRepository<User> UserRepo,
            CommercialLinkRepository clRepo
        ) : base(repo, utils)
        {
            this.UserRepo = UserRepo;
            this.ClRepo = clRepo;
        }

        public TEntity DTOToUserEntity(TDTO dto, int userId, int? id = null)
        {
            TEntity entity = this.Utils.Cast<TEntity, TDTO>(dto, id);
            entity.UserId = userId;
            return entity;
        }

        public override TDTO EntityToDTO(TEntity entity)
        {
            if (entity.User == null)
            {
                ((UserEntityRepository<TEntity>)this.repo).FillUserProp(entity);
            }
            TDTO ret = this.Utils.Cast<TDTO, TEntity>(entity, entity.Id);
            TDTO forUser = this.Utils.Cast<TDTO, User>(entity.User);
            IEnumerable<PropertyInfo> props = this.Utils.GetProps<TDTO>();
            foreach (PropertyInfo prop in props)
            {
                object value = prop.GetValue(forUser);
                if (value != null)
                {
                    prop.SetValue(ret, value);
                }
            }
            return ret;
        }

        public override TDTO Save(TDTO dto)
        {
            this.CheckDTO(dto, this.IsUnexpectedProp, this.IsRequiredProp);
            if (this.repo.FindAll().Any(entity => entity.ConnectionId == dto.ConnectionId))
            {
                throw new AppException($"'{dto.ConnectionId}' is already used as connectionId", 400);
            }
            User user = this.Utils.Cast<User, UserDTO>(dto);
            this.UserRepo.Save(user);
            TEntity entity = this.DTOToUserEntity(dto, user.Id);
            this.repo.Save(entity);
            return this.EntityToDTO(entity);
        }

        public override TDTO Update(int id, TDTO dto)
        {
            TEntity entity = this.FindEntity(id);
            this.CheckDTO(dto, this.IsUnexpectedProp, this.IsRequiredProp);
            if (this.repo.FindAll().Any(entity => entity.ConnectionId == dto.ConnectionId && entity.Id != id))
            {
                throw new AppException($"'{dto.ConnectionId}' is already used as connectionId", 400);
            }
            entity = this.repo.Update(this.DTOToUserEntity(dto, entity.UserId, id));
            entity.User = this.UserRepo.Update(this.Utils.Cast<User, UserDTO>(dto, entity.UserId));
            return this.EntityToDTO(entity);
        }

        public CommercialLinkDTO ClToDTO(CommercialLink cl)
        {
            CommercialLinkDTO ret = this.Utils.Cast<CommercialLinkDTO, CommercialLink>(cl);
            ret.Id = cl.Id;
            ret.Flags = new ClType()
            {
                Bookmark = this.Utils.GetBit(cl.Type, CommercialLink.BOOKMARK)
            };
            return ret;
        }
    }
}
