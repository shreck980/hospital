using MySqlConnector;

namespace hospital.DAO
{
    public abstract class LastIdGetter
    {

        protected string GetLastID;
        protected uint GetLastId(MySqlConnection connection, MySqlTransaction transaction)
        {
            uint id = 0;
            using (MySqlCommand command = new MySqlCommand(GetLastID, connection))
            {
                command.Transaction = transaction;
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    id = Convert.ToUInt32(result);
                }
            }

            return id;
        }

        protected uint GetLastId(MySqlConnection connection, MySqlTransaction transaction,string GetLastIdQuery)
        {
            uint id = 0;
            using (MySqlCommand command = new MySqlCommand(GetLastIdQuery, connection))
            {
                command.Transaction = transaction;
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    id = Convert.ToUInt32(result);
                }
            }

            return id;
        }

    }
}
