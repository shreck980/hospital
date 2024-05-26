using hospital.Entities;
using hospital.Exceptions;
using MySqlConnector;

namespace hospital.DAO.MySQL
{
    public class MySQLDrugDAO : LastIdGetter, IDrugDAO
    {
        DAOConfig config;

        private const string InsertDrug = "INSERT INTO drug (id, name, instruction) VALUES (@id, @name, @instruction);";
        private const string getAllDrugs = "select*from drug;";
        public MySQLDrugDAO(DAOConfig dAOConfig)
        {
            config = dAOConfig;
            GetLastID = "SELECT MAX(id) FROM drug";
        }

        public void AddDrug(List<Drug> drugs)
        {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {

                        using (var command = new MySqlCommand(InsertDrug, connection))
                        {
                            command.Transaction = transaction;

                            foreach (Drug d in drugs)
                            {
                                command.Parameters.Clear();
                                d.Id = GetLastId(connection, transaction) + 1;
                                command.Parameters.AddWithValue("@name", d.Name);
                                command.Parameters.AddWithValue("@id", d.Id);
                                command.Parameters.AddWithValue("@instruction", d.Instruction);

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

        public List<Drug> GetAllDrugs()
        {
            List<Drug> sList = new List<Drug>();

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                try
                {
                    connection.Open();
                    using (var command = new MySqlCommand(getAllDrugs, connection))
                    {


                        using var reader = command.ExecuteReader();
                        if (!reader.HasRows)
                        {
                            throw new NoSuchRecord("Нема жодного записаного препарату");
                        }
                        while (reader.Read())
                        {
                            Drug s = new Drug();
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
