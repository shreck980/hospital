using hospital.Entities;
using MySqlConnector;
using System.Data;

namespace hospital.DAO
{
    public class MySQLPatientDAO
    {



        private const string InsertPatientQuery = "INSERT INTO patient_account (id, name, surname, email, password, birthday, family_doctor, state, address) VALUES (@id, @name, @surname, @email, @password, @birthday, @family_doctor, @state, @address)";

        private const string UpdatePatientAccountStatus = "UPDATE patient_account SET state = @state where id = @id";
        DAOConfig config;
        private static string url;
        public MySQLPatientDAO(DAOConfig config)
        {
            this.config = config;
            //url = config.Url;
        }

        public void GetAllPatients()
        {

            // using var connection = new MySqlConnection(config.Url);

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                using var command = new MySqlCommand("SELECT* FROM patient_account;", connection);
                try
                {
                    connection.Open();
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Patient p = new Patient();
                        p.Name = (string)reader.GetValue(1);
                        p.Surname = (string)reader.GetValue(2);
                        p.Email = (string)reader.GetValue(3);
                        p.Password = (string)reader.GetValue(4);
                        p.Birthday = (DateTime)reader.GetValue(5);
                        p.State = (AccountStates)reader.GetValue(7);
                        Console.WriteLine(p);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            /*if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }*/

        }

        public Patient GetPatientByEmail(string email)
        {

            // using var connection = new MySqlConnection(config.Url);
            Patient p = new Patient();
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {

                using var command = new MySqlCommand("SELECT* FROM patient_account where email = @email", connection);
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@email", email);
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p.Id = (uint)reader.GetInt32(0);
                        p.Name = (string)reader.GetValue(1);
                        p.Surname = (string)reader.GetValue(2);
                        p.Email = (string)reader.GetValue(3);
                        p.Password = (string)reader.GetValue(4);
                        p.Birthday = (DateTime)reader.GetValue(5);
                        p.State = (AccountStates)reader.GetValue(7);
                        Console.WriteLine(p);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return p;


        }
        public Patient GetPatientById(uint id)
        {

            // using var connection = new MySqlConnection(config.Url);
            Patient p = new Patient();
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {

                using var command = new MySqlCommand("SELECT* FROM patient_account where id = @id", connection);
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p.Id = (uint)reader.GetInt32(0);
                        p.Name = (string)reader.GetValue(1);
                        p.Surname = (string)reader.GetValue(2);
                        p.Email = (string)reader.GetValue(3);
                        p.Password = (string)reader.GetValue(4);
                        p.Birthday = (DateTime)reader.GetValue(5);
                        p.State = (AccountStates)reader.GetValue(7);
                        p.FamilyDoctor.Id = reader.GetUInt32(6);
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

        public uint GetLastPatientId(MySqlConnection connection, MySqlTransaction transaction)
        {
            uint id = 1;
            string query = "SELECT MAX(id) FROM patient_account";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Transaction = transaction;
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    id = Convert.ToUInt32(result);
                }
                else
                {
                    throw new MySQLException("Error retrieving Patient information");


                }
            }

            return id;

        }


        public void AddPatientWithoutСonfirmation(Patient patient)
        {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        patient.Id = GetLastPatientId(connection, transaction) + 1;

                        using (var command = new MySqlCommand(InsertPatientQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddWithValue("@id", patient.Id);
                            command.Parameters.AddWithValue("@name", patient.Name);
                            command.Parameters.AddWithValue("@surname", patient.Surname);
                            command.Parameters.AddWithValue("@email", patient.Email);
                            command.Parameters.AddWithValue("@password", patient.Password);
                            command.Parameters.AddWithValue("@birthday", patient.Birthday);
                            command.Parameters.AddWithValue("@family_doctor", null);
                            command.Parameters.AddWithValue("@state", patient.State);
                            command.Parameters.AddWithValue("@address", patient.Address);

                            command.ExecuteNonQuery();

                        }
                        transaction.Commit();
                    }
                    catch (MySQLException e)
                    {
                        transaction.Rollback();
                        throw new MySQLException(e.Message, e);
                    }
                }
            }
        }

        public void UpdatePatientWithСonfirmation(Patient patient)
        {
            if (patient.State != AccountStates.Verified)
            {
                throw new Exception("Unverified account");
            }

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        //patient.Id = GetLastPatientId(connection, transaction);
                        using (var command = new MySqlCommand(UpdatePatientAccountStatus, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddWithValue("@state", patient.State);
                            command.Parameters.AddWithValue("@id", patient.Id);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (MySQLException e)
                    {
                        transaction.Rollback();
                        throw new MySQLException(e.Message, e);
                    }

                }
            }
        }
    }
}
