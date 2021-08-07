using System.IO;
using Microsoft.Extensions.Configuration;

namespace Import_Management.Helper
{
    public class Connection
    {
        //public static string iBOS { get; set; }        
        public static string iBOSDDD_VAT { get; internal set; }
        private static string GetConnection()
        {
            var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetDirectoryRoot(@"\"))
           .AddJsonFile("appSettings.json", false)
           .Build();
            var connString = config.GetSection("connectionString").Value;
            return connString;
        }
    }
}
