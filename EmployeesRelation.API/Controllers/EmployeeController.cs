using EmployeesRelation.API.Database;
using EmployeesRelation.API.Dto;
using EmployeesRelation.API.Filters;
using EmployeesRelation.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;

namespace EmployeesRelation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Logs _logger;
        public EmployeeController(Logs logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(Employee), 200)]
        [ProducesResponseType(204)]
        public IActionResult Get([FromQuery] int page, [FromQuery] int maxResults)
        {

            {
                var apikey = _configuration.GetValue<string>("AppInsightsInstrumantationKey");
                Console.WriteLine(apikey);

                List<Employee> database = JsonOperations.ReadEmployees();
                var pageData = database.OrderBy(x => x.Id).Skip((page - 1) * maxResults)
                                                        .OrderBy(x => x.Id)
                                                        .Take(maxResults)
                                                        .ToList();
                return Ok(pageData);
            }

        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Employee), 200)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            List<Employee> database = JsonOperations.ReadEmployees();
            var result = database.Where(x => x.Id == id).FirstOrDefault();
            if (result is null)
            {

                return NotFound("Employee not found.");
            }
            else
            {
                return Ok(result);
            }

        }

        [HttpPost]
        [Authorize(Roles = "Manager,HR,Junior")]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
        public IActionResult Post([FromBody] CreateEmployee request)
        {
            List<Employee> database = JsonOperations.ReadEmployees();
            var lastId = database.OrderBy(x => x.Id).Last().Id + 1;
            var employee = new Employee
            {
                Id = lastId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                JobTitle = request.JobTitle,
                Salary = request.Salary,

            };

            JsonOperations._database.Add(employee);
            JsonOperations.SaveEmployees();
            return Created(string.Empty, employee);
        }
        [HttpPost("request")]
        [Authorize]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByFilter([FromQuery] int page, [FromQuery] int maxResults, [FromBody] EmployeeParameters request)
        {
            List<Employee> database = JsonOperations.ReadEmployees();
            List<Employee> filteredData;
            if (request.Gender.Equals("string") && !request.JobTitle.Equals("string"))
            {
                filteredData = database
                                   .OrderBy(x => x.Id)
                                   .Where(x => x.JobTitle.Contains(request.JobTitle) && x.Salary >= request.Salary)
                                   .Skip((page - 1) * maxResults)
                                   .Take(maxResults)
                                   .ToList();
                return Ok(filteredData);
            }
            else if (request.JobTitle.Equals("string") && !request.Gender.Equals("string"))
            {

                filteredData = database
                                        .OrderBy(x => x.Id)
                                       .Where(x => x.Gender.ToUpper() == request.Gender.ToUpper() && x.Salary >= request.Salary)
                                       .Skip((page - 1) * maxResults)
                                       .Take(maxResults)
                                       .ToList();
                return Ok(filteredData);
            }
            else if (request.JobTitle.Equals("string") && request.Gender.Equals("string"))
            {
                filteredData = database
                                        .OrderBy(x => x.Id)
                                       .Where(x => x.Salary >= request.Salary)
                                       .Skip((page - 1) * maxResults)
                                       .Take(maxResults)
                                       .ToList();
                return Ok(filteredData);
            }
            else
            {
                filteredData = database
                                        .OrderBy(x => x.Id)
                                       .Where(x => x.JobTitle.Contains(request.JobTitle) && x.Gender.ToUpper() == request.Gender.ToUpper() && x.Salary >= request.Salary)
                                       .Skip((page - 1) * maxResults)
                                       .Take(maxResults)
                                       .ToList();
                return Ok(filteredData);
            }
            return NotFound("Employee not found.");

        }

        [HttpPut("{id}")]
        [CustomActionFilter()]
        [Authorize(Roles = "Manager,HR")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] CreateEmployee request)
        {
            List<Employee> database = JsonOperations.ReadEmployees();

            var found = database.Where(x => x.Id == id).FirstOrDefault();

            if(found is null)
            {
                return NotFound("Employee not found.");
            }

            JsonOperations._database.Remove(found);


            var inserted = new Employee
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                JobTitle = request.JobTitle,
                Salary = request.Salary,
            };


            JsonOperations._database.Add(inserted);
            _logger.EmployeeName = (inserted.FirstName.ToString() + " " + inserted.LastName.ToString());
            _logger.EmployeeId = id;
            _logger.Before = found.ToString();
            _logger.After = inserted.ToString();
            _logger.LoggedUserName = User.Identity.Name;
            JsonOperations.SaveEmployees();
            return Ok(inserted);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Manager,HR")]
        [CustomActionFilter()]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public IActionResult Patch([FromRoute] int id, EmployeeParameters request)
        {
            List<Employee> database = JsonOperations.ReadEmployees();

            var found = database.Where(x => x.Id == id).FirstOrDefault();

            if (found is null)
            {
                return NotFound("Employee not found.");
            }
            JsonOperations._database.Remove(found);

            var inserted = new Employee
            {
                Id = id,
                FirstName = found.FirstName,
                LastName = found.LastName,
                Gender = request.Gender,
                JobTitle = request.JobTitle,
                Salary = request.Salary,
            };
            JsonOperations._database.Add(inserted);
            JsonOperations.SaveEmployees();
            _logger.EmployeeName = (inserted.FirstName.ToString() + " " + inserted.LastName.ToString());
            _logger.EmployeeId = id;
            _logger.Before = found.ToString();
            _logger.After = inserted.ToString();
            _logger.LoggedUserName = User.Identity.Name;
            return Ok(inserted);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,HR")]
        [CustomActionFilter()]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            List<Employee> database = JsonOperations.ReadEmployees();
            var result = database.Where(x => x.Id == id).FirstOrDefault();
            if (result is null)
            {
                return NotFound("Employee not found.");
            }
            else
            {
                _logger.EmployeeName = (result.FirstName.ToString() + " " + result.LastName.ToString());
                _logger.EmployeeId = result.Id;
                _logger.Before = result.ToString();
                _logger.After = null;
                _logger.LoggedUserName = User.Identity.Name;
                JsonOperations._database.Remove(result);
                JsonOperations.SaveEmployees();
                return Ok(result);
            }
        }
    }
}