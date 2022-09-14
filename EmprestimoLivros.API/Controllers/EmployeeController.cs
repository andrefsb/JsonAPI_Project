using EmployeesRelation.API.Database;
using EmployeesRelation.API.Dto;
using EmployeesRelation.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;

namespace EmployeesRelation.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {

        private readonly ILogger<EmployeeController> _logger;


        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;

        }

        [HttpGet]
        [ProducesResponseType(typeof(Employee), 200)]
        [ProducesResponseType(204)]
        public IActionResult Get([FromQuery] int page, [FromQuery] int maxResults)
        {

            {
                List<Employee> database = JsonOperations.Read();
                var pageData = database.Skip((page - 1) * maxResults)
                                                        .Take(maxResults)
                                                        .OrderBy(x => x.Id)
                                                        .ToList();
                return Ok(pageData);
            }

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Employee), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            List<Employee> database = JsonOperations.Read();
            var result = database.Where(x => x.Id == id).FirstOrDefault();
            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
        [ProducesResponseType(204)]
        public IActionResult Post([FromBody] CreateEmployee request)
        {
            List<Employee> database = JsonOperations.Read();
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
            JsonOperations.Save();
            return Created(string.Empty, employee);
        }
        [HttpPost("request")]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByFilter([FromQuery] int page, [FromQuery] int maxResults, [FromBody] EmployeeParameters request)
        {
            List<Employee> database = JsonOperations.Read();
            List<Employee> filteredData;
            if (request.Gender == "string" && request.JobTitle != "string")
            {
                filteredData = database
                                   .Where(x => x.JobTitle.Contains(request.JobTitle) && x.Salary >= request.Salary)
                                   .Skip((page - 1) * maxResults)
                                   .Take(maxResults)
                                   .OrderBy(x => x.Id)
                                   .ToList();
            }
            else if (request.JobTitle == "string" && request.Gender != "string")
            {

                filteredData = database
                                       .Where(x => x.Gender.ToUpper() == request.Gender.ToUpper() && x.Salary >= request.Salary)
                                       .Skip((page - 1) * maxResults)
                                       .Take(maxResults)
                                       .OrderBy(x => x.Id)
                                       .ToList();
            }
            else if (request.JobTitle == "string" && request.Gender == "string")
            {
                filteredData = database
                                       .Where(x => x.Salary >= request.Salary)
                                       .Skip((page - 1) * maxResults)
                                       .Take(maxResults)
                                       .OrderBy(x => x.Id)
                                       .ToList();
            }
            else
            {
                filteredData = database
                                       .Where(x => x.JobTitle.Contains(request.JobTitle) && x.Gender.ToUpper() == request.Gender.ToUpper() && x.Salary >= request.Salary)
                                       .Skip((page - 1) * maxResults)
                                       .Take(maxResults)
                                       .OrderBy(x => x.Id)
                                       .ToList();
            }
            return Ok(filteredData);

        }

        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] CreateEmployee request)
        {
            List<Employee> database = JsonOperations.Read();

            var found = database.Where(x => x.Id == id).FirstOrDefault();
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
            JsonOperations.Save();
            return Ok(inserted);
        }


        [HttpPatch("{id}")]
        public IActionResult Patch()
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Employee), 200)]
        [ProducesResponseType(204)]
        public IActionResult Delete(int id)
        {
            List<Employee> database = JsonOperations.Read();
            var result = database.Where(x => x.Id == id).FirstOrDefault();
            JsonOperations._database.Remove(result);
            JsonOperations.Save();
            return Ok("Employee Deleted");
        }
    }
}