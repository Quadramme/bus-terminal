using System.Collections.Generic;

namespace Interfaces.Repository
{

    public interface IRepository<T> where T : class
    {
        void Create(T item);
        T Get(params object[] keyValues);
        IEnumerable<T> GetAll();
        void Update(T item);
        void Delete(params object[] keyValues);

    }

}