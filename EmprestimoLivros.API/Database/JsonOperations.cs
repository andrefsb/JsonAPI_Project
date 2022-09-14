using EmployeesRelation.API.Models;
using System.Text.Json;

namespace EmployeesRelation.API.Database
{
    public class JsonOperations
    {
        public static List<Employee> _database;

        public static List<Employee> Read()
        {
            using var reader = new StreamReader("./data.json");
            var json = reader.ReadToEnd();
            return _database = JsonSerializer.Deserialize<List<Employee>>(json);
        }

        public static void Save()
        {
            var content = JsonSerializer.Serialize(_database);
            System.IO.File.WriteAllText("./data.json", content);     
        }
    }
}
