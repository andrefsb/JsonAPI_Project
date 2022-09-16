using EmployeesRelation.API.Models;
using System.Text.Json;

namespace EmployeesRelation.API.Database
{
    public class JsonOperations
    {
        public static List<Employee> _database;
        public static List<Logs> _databaseEntry;

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

        public static List<Logs> ReadLog()
        {
            using var reader = new StreamReader("./datalogs.json");
            var json = reader.ReadToEnd();
            return _databaseEntry = JsonSerializer.Deserialize<List<Logs>>(json);
        }

        public static void SaveLog(List<Logs>_databaseEntry)
        {
            var content = JsonSerializer.Serialize(_databaseEntry);
            System.IO.File.WriteAllText("./datalogs.json", content);
        }
    }
}
