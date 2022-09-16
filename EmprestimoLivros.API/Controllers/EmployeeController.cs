using EmployeesRelation.API.Database;
using EmployeesRelation.API.Dto;
using EmployeesRelation.API.Filters;
using EmployeesRelation.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace EmployeesRelation.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly Logs _logger;
        public EmployeeController(Logs logger)
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
            if (result is null)
            {

                return StatusCode(StatusCodes.Status404NotFound);
            }
            else
            {
                return Ok(result);
            }

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
            //_logger.Before = employee.ToString();
            //_logger.After = null;
            JsonOperations._database.Add(employee);
            JsonOperations.Save();
            return Created(string.Empty, employee);
        }
        [HttpPost("request")]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByFilter([FromQuery] int page, [FromQuery] int maxResults, [FromBody] EmployeeParameters request)//Passar Body
        {
            List<Employee> database = JsonOperations.Read();
            List<Employee> filteredData;
            if (request.Gender.Equals("string") && !request.JobTitle.Equals("string"))
            {
                filteredData = database
                                   .Where(x => x.JobTitle.Contains(request.JobTitle) && x.Salary >= request.Salary)
                                   .Skip((page - 1) * maxResults)
                                   .Take(maxResults)
                                   .OrderBy(x => x.Id)
                                   .ToList();
            }
            else if (request.JobTitle.Equals("string") && !request.Gender.Equals("string"))
            {

                filteredData = database
                                       .Where(x => x.Gender.ToUpper() == request.Gender.ToUpper() && x.Salary >= request.Salary)
                                       .Skip((page - 1) * maxResults)
                                       .Take(maxResults)
                                       .OrderBy(x => x.Id)
                                       .ToList();
            }
            else if (request.JobTitle.Equals("string") && request.Gender.Equals("string"))
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
        [CustomActionFilter()]
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
            _logger.EmployeeName = (inserted.FirstName.ToString() + " " + inserted.LastName.ToString());
            _logger.EmployeeId = id;
            _logger.Before = found.ToString();
            _logger.After = inserted.ToString();
            JsonOperations.Save();
            return Ok(inserted);
        }

        [HttpPatch("{id}")]
        [CustomActionFilter()]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status404NotFound)]
        public IActionResult Patch([FromRoute] int id, EmployeeParameters request)
        {
            List<Employee> database = JsonOperations.Read();

            var found = database.Where(x => x.Id == id).FirstOrDefault();

            if (found is null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
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
            JsonOperations.Save();
            _logger.EmployeeName = (inserted.FirstName.ToString() + " " + inserted.LastName.ToString());
            _logger.EmployeeId = id;
            _logger.Before = found.ToString();
            _logger.After = inserted.ToString();
            return Ok(inserted);
        }

        [HttpDelete("{id}")]
        [CustomActionFilter()]
        [ProducesResponseType(typeof(Employee), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            List<Employee> database = JsonOperations.Read();
            var result = database.Where(x => x.Id == id).FirstOrDefault();
            if (result is null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            else
            {
                _logger.EmployeeName = (result.FirstName.ToString() + " " + result.LastName.ToString());
                _logger.EmployeeId = result.Id;
                _logger.Before = result.ToString();
                _logger.After = null;
                JsonOperations._database.Remove(result);
                JsonOperations.Save();
                return Ok(result);
            }
        }
    }
}