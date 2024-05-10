using hospital.Entities;
using MySqlConnector;

namespace hospital.DAO
{
    public class MySQLPaymentDAO
    {
        DAOConfig config;
        public MySQLPaymentDAO(DAOConfig dAOConfig)
        {
            config = dAOConfig;
        }

        public Payment? GetPaymentById(uint id)
        {

            // using var connection = new MySqlConnection(config.Url);
            Payment? p = null;
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {

                using var command = new MySqlCommand("SELECT* FROM payment_info where id = @id", connection);
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p.Id = reader.GetUInt32(0);
                        p.Price = reader.GetDecimal(1);
                        p.DateIssued = reader.GetDateTime(2);
                        p.DatePaid = reader.GetDateTime(3);
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
