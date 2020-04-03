using System.Collections.Generic;

namespace fidelizPlus_back.Services
{
    using DTO;

    public interface Service<T> where T : DTO
    {
        public IEnumerable<T> FindAll();

        public T Save(T dto);

        public T FindById(int id);

        public void Delete(int id);

        public T Update(int id, T dto);

        public IEnumerable<T> Filter(string filter);
    }
}
