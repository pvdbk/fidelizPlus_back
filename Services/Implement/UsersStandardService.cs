using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back.Services
{
    using DTO;
    using Models;
    using Repositories;

    public abstract class UsersStandardService<TEntity, TDTO> where TEntity : Entity, UserEntity where TDTO : UserDTO, new()
    {
        protected Repository<User> userRepo;
        protected UserEntityRepository<TEntity> entityRepo;
        protected CommercialLinkRepository commercialLinkRepo;

        public UsersStandardService(
            Repository<User> userRepo,
            UserEntityRepository<TEntity> entityRepo,
            CommercialLinkRepository commercialLinkRepo
        )
        {
            this.userRepo = userRepo;
            this.entityRepo = entityRepo;
            this.commercialLinkRepo = commercialLinkRepo;
        }

        public abstract bool IsRequiredProp(string prop);

        public abstract TDTO ToDTO(TEntity entity);

        public abstract TEntity DTOToEntity(int? id, int userId, TDTO dto);

        public abstract void Delete(int id);

        public static User DTOToUser(int? id, UserDTO dto)
        {
            User user = new User()
            {
                Surname = dto.Surname,
                FirstName = dto.FirstName,
                Email = dto.Email,
                Password = dto.Password
            };
            if (id != null)
            {
                user.Id = (int)id;
            }
            return user;
        }

        public TEntity Find(int id)
        {
            TEntity entity = this.entityRepo.FindById(id);
            if (entity == default(TEntity))
            {
                throw new AppException($"{typeof(TEntity).Name} not found", 404);
            }
            return entity;
        }

        public IEnumerable<TDTO> FindAll()
        {
            IEnumerable<TEntity> entities = this.entityRepo.FindAll().ToList();
            return entities.Select(this.ToDTO);
        }

        public TDTO FindById(int id)
        {
            return this.ToDTO(this.Find(id));
        }

        public IEnumerable<TDTO> Filter(string filter)
        {
            return this.entityRepo.Filter(new Tree(filter)).Select(this.ToDTO);
        }

        public TDTO Save(TDTO dto)
        {
            Utils.CheckDTO(dto, this.IsRequiredProp);
            if (this.entityRepo.FindAll().Any(entity => entity.ConnectionId == dto.ConnectionId))
            {
                throw new AppException($"'{dto.ConnectionId}' is already used as connectionId", 400);
            }
            User user = DTOToUser(null, dto);
            this.userRepo.Save(user);
            TEntity entity = this.DTOToEntity(null, user.Id, dto);
            this.entityRepo.Save(entity);
            return this.ToDTO(entity);
        }

        public TDTO Update(int id, TDTO dto)
        {
            TEntity entity = this.Find(id);
            Utils.CheckDTO(dto, this.IsRequiredProp);
            if (this.entityRepo.FindAll().Any(entity => entity.ConnectionId == dto.ConnectionId && entity.Id != id))
            {
                throw new AppException($"'{dto.ConnectionId}' is already used as connectionId", 400);
            }
            entity = this.entityRepo.Update(this.DTOToEntity(id, entity.UserId, dto));
            entity.User = this.userRepo.Update(DTOToUser(entity.UserId, dto));
            return this.ToDTO(entity);
        }
    }
}
