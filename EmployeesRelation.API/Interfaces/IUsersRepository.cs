using EmployeesRelation.API.Models;

namespace EmployeesRelation.API.Interfaces
{
    public interface IUsersRepository
    {
        Task<List<Users>> Get(int page, int maxResults);

        Task<Users?> Get(string username, string password);

        Task<Users> Insert(Users user);
    }
}
