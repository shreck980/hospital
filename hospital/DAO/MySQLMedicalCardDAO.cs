using hospital.Entities;
using MySqlConnector;
using System.Data;

namespace hospital.DAO
{
    public class MySQLMedicalCardDAO
    {
        DAOConfig config;
        // private static string url =
        private const string InsertMedicalCard = "INSERT INTO medical_card (id, patient) VALUES (@id,@patient)";
        private const string GetMaxId = "SELECT MAX(id) FROM medical_card";


        public MySQLMedicalCardDAO(DAOConfig config)
        {
            this.config = config;

        }

        public static uint GetLastMedicalCardId(MySqlConnection connection, MySqlTransaction transaction)
        {
            uint id = 1;

            //string query = ;



            using (MySqlCommand command = new MySqlCommand(GetMaxId, connection))
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

        public void AddMedicalCard(MedicalCard medicalCard, uint patientId)
        {

            if (medicalCard == null)
            {
                throw new ArgumentNullException(nameof(medicalCard), "Medical card cannot be null");
            }

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        medicalCard.Id = GetLastMedicalCardId(connection, transaction) + 1;
                        using (var command = new MySqlCommand(InsertMedicalCard, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddWithValue("@id", medicalCard.Id);
                            command.Parameters.AddWithValue("@patient", patientId);
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
