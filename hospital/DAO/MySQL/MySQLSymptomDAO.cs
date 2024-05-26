using hospital.Entities;
using hospital.Exceptions;
using hospital.Services;
using MySqlConnector;

namespace hospital.DAO.MySQL
{
    public class MySQLSymptomDAO : LastIdGetter, ISymptomDAO
    {
        DAOConfig config;

        private const string InsertSymptom = "INSERT INTO symptom (id, name, description) VALUES (@id, @name, @description); ";
        private const string getAllSymptoms = "select*from symptom; ";
        public MySQLSymptomDAO(DAOConfig dAOConfig)
        {
            config = dAOConfig;
            GetLastID = "SELECT MAX(id) FROM symptom";
        }

        public void AddSymptom(List<Symptom> symptoms)
        {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {

                        using (var command = new MySqlCommand(InsertSymptom, connection))
                        {


                            command.Transaction = transaction;
                            foreach (Symptom s in symptoms)
                            {
                                command.Parameters.Clear();
                                s.Id = GetLastId(connection, transaction) + 1;
                                command.Parameters.AddWithValue("@name", s.Name);
                                command.Parameters.AddWithValue("@id", s.Id);
                                command.Parameters.AddWithValue("@description", s.Description);

                                command.ExecuteNonQuery();
                            }
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

        public List<Symptom> GetAllSymptoms()
        {
            List<Symptom> sList = new List<Symptom>();

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                try
                {
                    connection.Open();
                    using (var command = new MySqlCommand(getAllSymptoms, connection))
                    {


                        using var reader = command.ExecuteReader();
                        if (!reader.HasRows)
                        {
                            throw new NoSuchRecord("Нема жодного записаного симптому");
                        }
                        while (reader.Read())
                        {
                            Symptom s = new Symptom();
                            s.Id = reader.GetUInt32(0);
                            s.Name = reader.GetString(1);
                            sList.Add(s);
                        }



                    }
                    return sList;

                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);

                }
            }
        }
    }
}
