using EmployeesRelation.API.AuthorizationAndAuthentication;
using EmployeesRelation.API.Database;
using EmployeesRelation.API.Dto;
using EmployeesRelation.API.Interfaces;
using EmployeesRelation.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesRelation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly GenerateToken _generateToken;
        public UsersController(GenerateToken generateToken)
        {
            _generateToken = generateToken;
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public IActionResult Get([FromQuery] int page, [FromQuery] int maxResults)
        {
            List<Users> list = JsonOperations.ReadUsers();
            var users = list.Skip((page - 1) * maxResults)
                                                            .OrderBy(x => x.Id)
                                                            .Take(maxResults)
                                                            .ToList();
            return Ok(users);
        }

        [HttpPost]
        
        public IActionResult GetUserByUserPass([FromBody] UsersDto usersDto)
        {
            List<Users> list = JsonOperations.ReadUsers();
            var user = list.FirstOrDefault(item => item.UserName.Equals(usersDto.UserName) && item.Password.Equals(usersDto.Password));

            if (user is null)
            {
                return NotFound();
            }
            return Created("",user);
        }

        [HttpPost("create")]
        [Authorize(Roles ="Manager")]
        public IActionResult InsertUser([FromBody] UsersDto usersDto)
        {
            List<Users> list = JsonOperations.ReadUsers();
            var newUser = new Users();
            var lastId = list.OrderBy(x => x.Id).Last().Id + 1;
            newUser.Id = lastId;
            newUser.Name = usersDto.Name;
            newUser.UserName = usersDto.UserName;
            newUser.Password = usersDto.Password;
            newUser.Role = usersDto.Role;
            list.Add(newUser);
            JsonOperations.SaveUsers(list);
            return Ok(newUser);
        }


        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] Authenticate authInfo)
        {
            List<Users> list = JsonOperations.ReadUsers();
            var user = list.FirstOrDefault(item => item.UserName.Equals(authInfo.Username) && item.Password.Equals(authInfo.Password));

            if (user is null)
            {
                return NotFound(new { message = "Invalid login and/or password." });
            }

            var token = _generateToken.GenerateJwt(user);
            user.Password = "";
            return Ok(new {user = user, token = token});
        }
    }
}
