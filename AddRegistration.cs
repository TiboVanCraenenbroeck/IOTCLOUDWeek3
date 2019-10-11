using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using oef1.Models;

namespace oef1
{
    public static class Function1
    {
        [FunctionName("AddRegistration")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/registrations")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

                string json = await new StreamReader(req.Body).ReadToEndAsync();
                Registration newRegistration = JsonConvert.DeserializeObject<Registration>(json);
                newRegistration.RegistrationID = Guid.NewGuid().ToString();
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO Registrations VALUES(@regid, @LastName, @FirstName,@Email, @ZipCode, @Age, @IstFirstName)";
                        command.Parameters.AddWithValue("@regid", newRegistration.RegistrationID);
                        command.Parameters.AddWithValue("@LastName", newRegistration.LastName);
                        command.Parameters.AddWithValue("@FirstName", newRegistration.FirstName);
                        command.Parameters.AddWithValue("@ZipCode", newRegistration.ZipCode);
                        command.Parameters.AddWithValue("@Age", newRegistration.Age);
                        command.Parameters.AddWithValue("@Email", newRegistration.Email);
                        command.Parameters.AddWithValue("@IstFirstName", newRegistration.IsFirstTimer);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                return new OkObjectResult(newRegistration);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "AddRegistration");
                return new StatusCodeResult(500);
            }
        }
    }
}
/*
 * {
	"LastName":"vc",
	"FirstName":"Tibo",
	"ZipCode": "8500",
	"Age": 13,
	"Email": "test@tibo.be",
	"IsFirstTimer":true
}
*/
