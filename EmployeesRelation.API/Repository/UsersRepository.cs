//using EmployeesRelation.API.Database;
//using EmployeesRelation.API.Interfaces;
//using EmployeesRelation.API.Models;
//using System.Collections.Generic;

//namespace EmployeesRelation.API.Repository
//{
//    public class UsersRepository : IUsersRepository
//    {
//        List<Users> usersList = new List<Users>();



//        public Task<List<Users>> Get(int page, int maxResults)
//        {
//            return Task.Run(() =>
//            {
//                var users = usersList.Skip((page - 1) * maxResults)
//                                                            .OrderBy(x => x.Id)
//                                                            .Take(maxResults)
//                                                            .ToList();
//                return users.Any() ? users : new List<Users>();
//            });
//        }
//        public Task<Users> Get(string username, string password)
//        {
//            return Task.Run(() =>
//            {

//                var user = usersList.FirstOrDefault(item => item.UserName.Equals(username) && item.Password.Equals(password));

//                return user;
//            });
//        }

//        public Task<Users> Insert(Users user)
//        {
//            return Task.Run(() =>
//            {
//                usersList.Add(user);
//                JsonOperations.SaveUsers(usersList);
//                return user;
//            });
//        }

//    }
//}
