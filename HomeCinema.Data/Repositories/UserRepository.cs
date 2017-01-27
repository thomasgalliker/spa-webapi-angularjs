using System.Linq;

using EntityFramework.Toolkit;
using EntityFramework.Toolkit.Core;

using HomeCinema.Entities;

namespace HomeCinema.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(IHomeCinemaContext context)
         : base(context)
        {
        }

        public User GetByUsername(string username)
        {
            //return this.Get().SingleOrDefault(u => u.Username == username);
            return this.FindBy(u => u.Username == username).SingleOrDefault();
        }
    }

    public interface IUserRepository : IGenericRepository<User>
    {
        User GetByUsername(string username);
    }
}
