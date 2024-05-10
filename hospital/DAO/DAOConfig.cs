using MySqlConnector;

namespace hospital.DAO
{
    public class DAOConfig
    {

        public string Server { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string Url { get; set; }
        public string DatabaseType { get; set; }

        public DAOConfig()
        {
            var databaseConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings");
            var _server = databaseConfig["Server"];
            if (_server != null)
            {
                Server = _server;
            }
            var _port = databaseConfig["Port"];
            if (_port != null)
            {
                Port = _port;
            }

            var _user = databaseConfig["User"];
            if (_user != null)
            {
                User = _user;
            }
            var _password = databaseConfig["Password"];

            if (_password != null)
            {
                Password = _password;
            }

            var _database = databaseConfig["Database"];

            if (_database != null)
            {
                Database = _database;
            }

            var _databaseType = databaseConfig["DatabaseType"];

            if (_databaseType != null)
            {
                DatabaseType = _databaseType;
            }

            Url = $"Server={Server};Port={Port};User ID={User};Password={Password};Database={Database}";

        }
    }
}
