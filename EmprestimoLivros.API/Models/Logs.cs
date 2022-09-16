namespace EmployeesRelation.API.Models
{
    public class Logs
    {
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Before { get; set; }
        public string After { get; set; }


        public void Write()
        {
            if (After is null && EmployeeId is not 0)
            {
                Console.WriteLine($"{Name} - {Date:dd/MM/yyyy - hh:mm:ss} - Employee {EmployeeId} - {EmployeeName} - Removido.");
            }
            else if (After is not null)
            {
                Console.WriteLine($"{Name} - {Date:dd/MM/yyyy - hh:mm:ss} - Employee {EmployeeId} - {EmployeeName} - Alterado de {Before} para {After}.");
            }
            else
            {
                Console.WriteLine("Operation aborted.");
            }
        }
    }
}
