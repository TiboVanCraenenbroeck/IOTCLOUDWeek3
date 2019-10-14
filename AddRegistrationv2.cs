using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using oef1.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using oef1.Enteties;

namespace oef1
{
    public static class AddRegistrationv2
    {
        private static object cloudTableClient;

        [FunctionName("AddRegistrationv2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v2/registrations")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string connectionString = Environment.GetEnvironmentVariable("AzureStorage");

                //Ophalen van de data
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                Registration newRegistration = JsonConvert.DeserializeObject<Registration>(json);
                newRegistration.RegistrationID = Guid.NewGuid().ToString();

                // Hoe noem je methode parse?
                // = Static methode --> Je maakt geen cloud-storage object aan --> Vraagt gewoon functie op
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("registrations");

                // Wegschrijven
                // Partitionkey --> Goed over nadenken welke je gaat kiezen
                RegistrationEntety ent = new RegistrationEntety(newRegistration.ZipCode, newRegistration.RegistrationID)
                {
                    LastName = newRegistration.LastName,
                    FirstName = newRegistration.FirstName,
                    Email = newRegistration.Email,
                    Age = newRegistration.Age,
                    IsFirstTimer = newRegistration.IsFirstTimer,
                    ZipCode = newRegistration.ZipCode,
                    RegistrationID = newRegistration.RegistrationID
                };
                TableOperation insertOperation = TableOperation.Insert(ent);
                await table.ExecuteAsync(insertOperation);
                // Mail versturen
                Mailer mailsender = new Mailer();
                mailsender.SendMail(newRegistration);
                return new OkObjectResult(newRegistration);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "AddRegistrationV2");
                return new StatusCodeResult(500);
            }
        }
    }
}
