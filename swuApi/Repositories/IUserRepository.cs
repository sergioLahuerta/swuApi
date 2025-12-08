using swuApi.Models;

namespace swuApi.Repositories
{
    public interface IUserRepository : IRepository<User> 
    {
        Task<User?> GetByUsernameOrEmailAsync(string username, string email);
    }
}