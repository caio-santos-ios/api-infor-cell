using api_infor_cell.src.Models;
using MongoDB.Driver;

namespace api_infor_cell.src.Configuration
{
    public class AppDbContext
    {
        public static string? ConnectionString { get; set; }
        public static string? DatabaseName { get; set; }
        public static bool IsSSL { get; set; }
        private IMongoDatabase Database { get; }

        public AppDbContext()
        {
            try
            {
                MongoClientSettings mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
                if (IsSSL)
                {
                    mongoClientSettings.SslSettings = new SslSettings
                    {
                        EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                    };
                }
                
                var mongoClient = new MongoClient(mongoClientSettings);
                Database = mongoClient.GetDatabase(DatabaseName);
            }
            catch(Exception ex)
            {
                throw new Exception($"Failed to connect to database. Error: {ex.Message}");
            }
        }

        #region MASTER DATA
        public IMongoCollection<Company> Companies
        {
            get { return Database.GetCollection<Company>("companies"); }
        }
        public IMongoCollection<User> Users
        {
            get { return Database.GetCollection<User>("users"); }
        }
        public IMongoCollection<GenericTable> GenericTables
        {
            get { return Database.GetCollection<GenericTable>("generic_tables"); }
        }
        public IMongoCollection<Address> Addresses
        {
            get { return Database.GetCollection<Address>("addresses"); }
        }
        public IMongoCollection<Contact> Contacts
        {
            get { return Database.GetCollection<Contact>("contacts"); }
        }
        public IMongoCollection<Attachment> Attachments
        {
            get { return Database.GetCollection<Attachment>("attachments"); }
        }
        public IMongoCollection<Supplier> Suppliers
        {
            get { return Database.GetCollection<Supplier>("suppliers"); }
        }
        #endregion
    }
}