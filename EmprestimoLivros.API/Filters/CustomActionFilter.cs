using EmployeesRelation.API.Database;
using EmployeesRelation.API.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeesRelation.API.Filters
{
    public class CustomActionFilter : Attribute, IActionFilter
    {
        Logs log;
        public static List<Logs> datasaved;

        public void OnActionExecuted(ActionExecutedContext context)
        {
            datasaved = JsonOperations.ReadLog();
            log.Name = context.HttpContext.Request.Method;
            if (datasaved[0].EmployeeId==0)
            {
                datasaved.Clear();
                datasaved.Add(log);
            }
            else
            {
                datasaved.Add(log);
            }
            JsonOperations.SaveLog(datasaved);
            log.Write();   
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            log = context.HttpContext.RequestServices.GetService<Logs>();
            log.Date  = DateTime.Now;
        }
    }
}
