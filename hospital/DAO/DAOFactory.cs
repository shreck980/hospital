namespace hospital.DAO
{
    public class DAOFactory
    {
        private readonly IServiceProvider serviceProvider;
        public DAOFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public MySQLPatientDAO? getPatientDAO()
        {
            return (MySQLPatientDAO?)serviceProvider.GetService(typeof(MySQLPatientDAO));
        }
    }
}
