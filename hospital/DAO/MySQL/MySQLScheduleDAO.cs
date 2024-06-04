using hospital.Entities;
using hospital.Exceptions;
using MySqlConnector;
using System.Numerics;

namespace hospital.DAO.MySQL
{
    public class MySQLScheduleDAO : LastIdGetter,IScheduleDAO
    {
        DAOConfig config;

        private const string InsertSchedule = "INSERT INTO schedule (id, time_start, time_end,doctor,appointment) VALUES (@id, @time_start, @time_end, @doctor, @appointment) ";
        public MySQLScheduleDAO(DAOConfig dAOConfig)
        {
            config = dAOConfig;
            GetLastID = "SELECT MAX(id) FROM schedule";
        }

        public void AddSchedule(List<Event> schedule, long doctor)
        {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {

                        using (var command = new MySqlCommand(InsertSchedule, connection))
                        {


                            command.Transaction = transaction;
                            foreach (Event e in schedule)
                            {
                                command.Parameters.Clear();
                                e.Id = GetLastId(connection, transaction) + 1;

                                command.Parameters.AddWithValue("@id", e.Id);
                                command.Parameters.AddWithValue("@time_start", e.Start);
                                command.Parameters.AddWithValue("@time_end", e.End);
                                command.Parameters.AddWithValue("@doctor", doctor);
                                command.Parameters.AddWithValue("@appointment", e.Appointment == null ? DBNull.Value : e.Appointment.Id);

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

        public List<Event> GetScheduleByDoctorIdForPatient(long doctor)
        {
            List<Event> schedule = new List<Event>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(config.Url))
                {

                    using (var command = new MySqlCommand("SELECT* FROM schedule where doctor = @doctor_id and appointment is  null;", connection))
                    {
                        connection.Open();
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@doctor_id", doctor);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Event e = new Event();
                                e.Id = reader.GetInt64(0);
                                e.Start = reader.GetDateTime(1);
                                e.End = reader.GetDateTime(2);
                                schedule.Add(e);

                            }
                        }
                    }
                }
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }


            return schedule;

        }


        public List<Event> GetScheduleByDoctorIdForDoctor(long doctor)
        {
            List<Event> schedule = new List<Event>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(config.Url))
                {

                    using (var command = new MySqlCommand("SELECT * FROM schedule WHERE doctor = @doctor_id AND appointment IS NOT NULL AND appointment IN (SELECT id FROM appointment WHERE state in (1,2) or state=5);", connection))
                    {
                        connection.Open();
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@doctor_id", doctor);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                return schedule;
                            }
                            while (reader.Read())
                            {
                                Event e = new Event();
                                e.Id = reader.GetInt64(0);
                                e.Start = reader.GetDateTime(1);
                                e.End = reader.GetDateTime(2);
                                e.Appointment = new Appointment();
                                e.Appointment.Id = reader.GetInt64(4);
                                schedule.Add(e);

                            }
                           
                        }
                    }
                }
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }


            return schedule;

        }


        public Event GetEvenById(long id)
        {
            Event ev = new Event();
            if (id == 0)
            {
                throw new MySQLException("Неможливо записати ваш вибір годин для прийому, спробуйте ще раз");
            }
            try
            {

                using (MySqlConnection connection = new MySqlConnection(config.Url))
                {

                    using (var command = new MySqlCommand("SELECT* FROM schedule where id = @id and appointment is null;", connection))
                    {
                        connection.Open();
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                ev.Id = reader.GetInt64(0);
                                ev.Start = reader.GetDateTime(1);



                            }
                        }
                    }
                }
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }


            return ev;
        }

        public void MarkEventAsBooked(long eventId, long appointmentId)
        {
            if (appointmentId == 0)
            {
                throw new MySQLException("Сталася помилка при збережені вашого запису на прийом. Будь ласка апробуйте ще раз");
            }
            try
            {

                using (MySqlConnection connection = new MySqlConnection(config.Url))
                {

                    using (var command = new MySqlCommand("Update schedule set appointment =@appointment_id where id = @id", connection))
                    {

                        command.Parameters.AddWithValue("@appointment_id", appointmentId);
                        command.Parameters.AddWithValue("@id", eventId);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }

        }
    }
}
