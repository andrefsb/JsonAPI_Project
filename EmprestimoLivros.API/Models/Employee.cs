using System.Text.Json.Serialization;

namespace EmployeesRelation.API.Models
{
    public class Employee
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("job_title")]
        public string JobTitle { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("salary")]
        public double Salary { get; set; }


        public override string ToString()
        {
            return $"Id:{Id} - Full Name: {FirstName} {LastName} - Gender: {Gender} - Job Title: {JobTitle} - ${Salary}";
        }
    }

}

