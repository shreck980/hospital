using hospital.Entities;
using MySqlConnector;

namespace hospital.DAO
{
    public class MySQLDoctorDAO
    {
        DAOConfig config;
        public MySQLDoctorDAO(DAOConfig config)
        {
            config = new DAOConfig();
        }



        public Doctor? GetDoctorById(uint id)
        {

            // using var connection = new MySqlConnection(config.Url);
            Doctor? d = null;
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {

                using var command = new MySqlCommand("SELECT* FROM doctor_account where id = @id", connection);
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        d.Id = reader.GetUInt32(0);
                        d.Name = reader.GetString(1);
                        d.Surname = reader.GetString(2);
                        d.Email = reader.GetString(3);
                        d.Password = reader.GetString(4);
                        d.Accessibility =(Accessibility)reader.GetInt16(5);
                        Console.WriteLine(d);

                    }
                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);
                }
            }

            return d;


        }
    }
}
