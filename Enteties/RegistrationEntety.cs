using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace oef1.Enteties
{
    public class RegistrationEntety: TableEntity
    {
        // Constructor
        public RegistrationEntety(string zipcode, string registrationId)
        {
            // Komt van TableEntity --> Deze var daarin steken
            this.PartitionKey = zipcode;
            this.RowKey = registrationId;
        }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string ZipCode { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public bool IsFirstTimer { get; set; }
        public string RegistrationID { get; internal set; }
    }
}
