using EmployeesRelation.API.Models;
using System.Text.Json;

namespace EmployeesRelation.API.Database
{
    public class JsonOperations
    {
        public static List<Employee> _database;
        public static List<Logs> _databaseEntry;
        public static List<Users> _databaseUsers;

        public static List<Employee> ReadEmployees()
        {
            using var reader = new StreamReader("./data.json");
            var json = reader.ReadToEnd();
            return _database = JsonSerializer.Deserialize<List<Employee>>(json);
        }

        public static void SaveEmployees()
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

        public static List<Users> ReadUsers()
        {
            using var reader = new StreamReader("./datausers.json");
            var json = reader.ReadToEnd();
            return _databaseUsers = JsonSerializer.Deserialize<List<Users>>(json);
        }

        public static void SaveUsers(List<Users> _databaseUsers)
        {
            var content = JsonSerializer.Serialize(_databaseUsers);
            System.IO.File.WriteAllText("./datausers.json", content);
        }
    }
}
