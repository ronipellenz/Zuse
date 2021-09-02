using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistance.Interfaces
{
    public interface IDbContext
    {
        Task<User> SaveUserAsync(User user);
        Task<User> GetByIdAsync(string id);
        Task<IEnumerable<User>> GetUsersListAsync();
        Task<User> UpdateAsync(User user, string partitionKey);
        Task<User> DeleteUserAsync(string id);
    }
}
