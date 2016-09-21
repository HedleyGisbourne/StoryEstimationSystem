using System.Data.Entity.Core.EntityClient;

namespace HG.SoftwareEstimationService.Repository
{
    public static class ConnectionStringManager
    {
        public static string GetConnectionString()
        {
            string database = @"C:\Users\Hedley\Desktop\Diss\SqliteData\Database.sqlite";
            string sqlLiteConnectionString = string.Format("data source=\"{0}\";datetimeformat=Ticks", database);

            return new EntityConnectionStringBuilder
            {
                Metadata = "res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl",
                Provider = "System.Data.SQLite.EF6",
                ProviderConnectionString = sqlLiteConnectionString,
            }.ConnectionString;
        }
    }
}