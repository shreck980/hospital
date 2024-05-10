using MySqlConnector;
using System.Data;
using hospital.Entities;

namespace hospital.DAO
{
    public class MySQLAppointmentDAO
    {
        DAOConfig config;
        public MySQLAppointmentDAO(DAOConfig dAOConfig)
        {
            config = dAOConfig;
        }
        private const string InsertAppointment = "INSERT INTO appointment (patient, doctor, time_start, room_number, id, state, reason_for_appeal, payment) VALUES(@patient, @doctor, @time_start, @room_number, @id, @state, @reason_for_appeal, @payment)";
        private const string GetLastMaxId = "SELECT MAX(id) FROM appointment";
        private const string getAppointmentByPatientAndTime = "SELECT* FROM appointment where patient = @patient and time_start = @time";



        public static uint GetLastAppointmentdId(MySqlConnection connection, MySqlTransaction transaction)
        {
            uint id = 1;

            

            using (MySqlCommand command = new MySqlCommand(GetLastMaxId, connection))
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


        public void AddApppointment(Appointment a)
        {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    
                    try
                    {
                        a.Id = GetLastAppointmentdId(connection, transaction)+1;

                        using (MySqlCommand cmd = new MySqlCommand(InsertAppointment)) {
                            cmd.Transaction = transaction;
                            cmd.Parameters.AddWithValue("@patient", a.Patient.Id);
                            cmd.Parameters.AddWithValue("@doctor", a.Doctor.Id);
                            cmd.Parameters.AddWithValue("@time_start", a.TimeStart);
                            cmd.Parameters.AddWithValue("@room_number", a.RoomNumber);
                            cmd.Parameters.AddWithValue("@id", a.Id);
                            cmd.Parameters.AddWithValue("@state", a.State);
                            cmd.Parameters.AddWithValue("@reason_for_appeal", a.ReasonForAppeal);
                            cmd.Parameters.AddWithValue("@payment", a.Payment);
                            cmd.ExecuteNonQuery();
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


        public Appointment? GetAppointmentByPatientAndTime(Patient p, DateTime time)
        {
            Appointment? a = null;

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                try
                {
                    connection.Open();
                    using (var command = new MySqlCommand(getAppointmentByPatientAndTime , connection))
                    {
                        command.Parameters.AddWithValue("@patient", p.Id);
                        command.Parameters.AddWithValue("@time", time);
                        using var reader = command.ExecuteReader();
                        if (reader != null)
                        {
                            while (reader.Read())
                            {

                                a.Id = reader.GetUInt32(4);
                                a.Patient.Id = reader.GetUInt32(0);
                                a.Doctor.Id = reader.GetUInt32(1);
                                a.TimeStart = reader.GetDateTime(2);
                                a.RoomNumber = reader.GetUInt32(3);
                                a.State = (AppointmentState)reader.GetInt32(5);
                                a.ReasonForAppeal = reader.GetString(6);
                                a.Payment.Id = reader.GetUInt32(7);

                            }
                            
                        }

                        //return a;
                    }
                   
                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);

                }
            }

            return a;
        }
    }
}
