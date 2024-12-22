using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class UserRepository : IRepository<User>
    {

        public UserRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(User item)
        {
            dbc.User.Add(item);
        }
        
        public User Get(params object[] keyValues)
        {
            return dbc.User.Find(keyValues);
        }

        public IEnumerable<User> GetAll()
        {
            return dbc.User.ToList();
        }

        public void Update(User item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.User.Find(keyValues);

            if (bc != null)
                dbc.User.Remove(bc);

        }

        private BusTerminalContext dbc;

    }

}