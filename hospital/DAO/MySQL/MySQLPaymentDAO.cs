using hospital.Entities;
using hospital.Exceptions;
using MySqlConnector;

namespace hospital.DAO.MySQL
{
    public class MySQLPaymentDAO : IPaymentDAO
    {
        DAOConfig config;
        public MySQLPaymentDAO(DAOConfig dAOConfig)
        {
            config = dAOConfig;
        }
        private const string SelectPaymentById = "SELECT* FROM payment_info where id = @id";
        public Payment? GetPaymentById(uint id)
        {

            // using var connection = new MySqlConnection(config.Url);
            Payment? p = null;
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {

                using var command = new MySqlCommand(SelectPaymentById, connection);
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p = new Payment();
                        p.Id = reader.GetUInt32(0);
                        p.Price = reader.GetDecimal(1);
                        p.DateIssued = reader.GetDateTime(2);
                        p.DatePaid = reader["date_paid"] == DBNull.Value ? null : reader.GetDateTime(3);
                        p.Patient.Id = reader.GetUInt32(4);
                        Console.WriteLine(p);

                    }
                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);
                }
            }

            return p;


        }
    }
}
