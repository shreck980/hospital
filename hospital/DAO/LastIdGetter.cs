using MySqlConnector;

namespace hospital.DAO
{
    public abstract class LastIdGetter
    {

        protected string GetLastID;
        protected long GetLastId(MySqlConnection connection, MySqlTransaction transaction)
        {
            long id = 0;
            using (MySqlCommand command = new MySqlCommand(GetLastID, connection))
            {
                command.Transaction = transaction;
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    id = Convert.ToInt64(result);
                }
            }

            return id;
        }

        protected long GetLastId(MySqlConnection connection, MySqlTransaction transaction,string GetLastIdQuery)
        {
            long id = 0;
            using (MySqlCommand command = new MySqlCommand(GetLastIdQuery, connection))
            {
                command.Transaction = transaction;
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    id = Convert.ToInt64(result);
                }
            }

            return id;
        }

    }
}
