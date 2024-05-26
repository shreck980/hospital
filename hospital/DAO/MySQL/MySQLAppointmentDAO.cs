using MySqlConnector;
using System.Data;
using hospital.Entities;
using hospital.Exceptions;

namespace hospital.DAO.MySQL
{
    public class MySQLAppointmentDAO : LastIdGetter, IAppointmentDAO
    {
        DAOConfig config;
        public MySQLAppointmentDAO(DAOConfig dAOConfig)
        {
            config = dAOConfig;
            GetLastID = "SELECT MAX(id) FROM appointment";
        }
        private const string InsertAppointment = "INSERT INTO appointment (patient, doctor, time_start, room_number, id, state, reason_for_appeal, payment) VALUES(@patient, @doctor, @time_start, @room_number, @id, @state, @reason_for_appeal, @payment);";
        private const string InsertPayment = "INSERT INTO payment_info (id, price, date_issued, date_paid, patient) VALUES(@id, @price, @date_issued, @date_paid, @patient);";
        //private const string GetLastMaxId = "SELECT MAX(id) FROM appointment";
        private const string getAppointmentByPatientAndTime = "SELECT* FROM appointment where patient = @patient and time_start = @time;";
        private const string getPatientAppointments = "SELECT* FROM appointment where state in (1, 2) and patient = @patient;";
        private const string getAllPatientAppointments = "SELECT* FROM appointment where  patient = @patient;";
        private const string getAppointmentById = "SELECT* FROM appointment where id = @id and state in(1,2);";
        private const string getDoctorAppointments = "SELECT* FROM appointment where doctor = @doctor;";
        private const string GetLastMaxIdPayment = "SELECT MAX(id) FROM payment_info;";


        /*  public  uint GetLastAppointmentdId(MySqlConnection connection, MySqlTransaction transaction)
          {
              uint id = 1;



              using (MySqlCommand command = new MySqlCommand(GetLastID, connection))
              {
                  command.Transaction = transaction;
                  var result = command.ExecuteScalar();
                  if (result != null && result != DBNull.Value)
                  {
                      id = Convert.ToUInt32(result);
                  }
                  else
                  {
                      throw new MySQLException("Error retrieving Appointment information");


                  }
              }
              return id;




          }*/

        /* public  uint GetLastPaymentdId(MySqlConnection connection, MySqlTransaction transaction)
         {
             uint id = 1;
             using (MySqlCommand command = new MySqlCommand(GetLastMaxIdPayment, connection))
             {
                 command.Transaction = transaction;
                 var result = command.ExecuteScalar();
                 if (result != null && result != DBNull.Value)
                 {
                     id = Convert.ToUInt32(result);
                 }
                 else
                 {
                     throw new MySQLException("Error retrieving Payment information");


                 }
             }
             return id;




         }*/


        public void AddApppointment(Appointment a)
        {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {

                    try
                    {
                        if (a.Payment != null)
                        {
                            a.Payment.Id = GetLastId(connection, transaction, GetLastMaxIdPayment) + 1;
                            using (MySqlCommand cmd = new MySqlCommand(InsertPayment, connection))
                            {
                                cmd.Transaction = transaction;

                                cmd.Parameters.AddWithValue("@id", a.Payment.Id);
                                cmd.Parameters.AddWithValue("@price", a.Payment.Price);
                                cmd.Parameters.AddWithValue("@date_issued", a.Payment.DateIssued);
                                cmd.Parameters.AddWithValue("@date_paid", a.Payment.DatePaid == null ? DBNull.Value : a.Payment.DatePaid);
                                cmd.Parameters.AddWithValue("@patient", a.Payment.Patient.Id);

                                cmd.ExecuteNonQuery();
                            }
                        }
                        a.Id = GetLastId(connection, transaction) + 1;

                        using (MySqlCommand cmd = new MySqlCommand(InsertAppointment, connection))
                        {
                            cmd.Transaction = transaction;
                            cmd.Parameters.AddWithValue("@patient", a.Patient.Id);
                            cmd.Parameters.AddWithValue("@doctor", a.Doctor.Id);
                            cmd.Parameters.AddWithValue("@time_start", a.TimeStart);
                            cmd.Parameters.AddWithValue("@room_number", a.RoomNumber);
                            cmd.Parameters.AddWithValue("@id", a.Id);
                            cmd.Parameters.AddWithValue("@state", a.State);
                            cmd.Parameters.AddWithValue("@reason_for_appeal", a.ReasonForAppeal);
                            cmd.Parameters.AddWithValue("@payment", a.Payment == null ? DBNull.Value : a.Payment.Id);
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


        public Appointment GetAppointmentByPatientAndTime(Patient p, DateTime time)
        {
            Appointment a = new Appointment();

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                try
                {
                    connection.Open();
                    using (var command = new MySqlCommand(getAppointmentByPatientAndTime, connection))
                    {
                        command.Parameters.AddWithValue("@patient", p.Id);
                        command.Parameters.AddWithValue("@time", time.ToString("yyyy-MM-dd HH:mm:ss"));
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                throw new NoSuchRecord("У вас ще немає прийомів");
                            }
                            while (reader.Read())
                            {
                                a = new Appointment();
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

        public Appointment GetAppointmentById(uint id)
        {
           

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                Appointment a = new Appointment();
                try
                {
                    connection.Open();
                    using (var command = new MySqlCommand(getAppointmentById, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        //command.Parameters.AddWithValue("@time", time.ToString("yyyy-MM-dd HH:mm:ss"));
                        using (var reader = command.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new NoSuchRecord("Помилка знайти запис на прийом до вас");
                            }

                            while (reader.Read())
                            {

                                a.Id = reader.GetUInt32(4);
                                a.Patient.Id = reader.GetUInt32(0);
                                a.Doctor.Id = reader.GetUInt32(1);
                                a.TimeStart = reader.GetDateTime(2);
                                a.RoomNumber = reader.GetUInt32(3);
                                a.State = (AppointmentState)reader.GetInt32(5);
                                a.ReasonForAppeal = reader.GetString(6);
                                a.Payment.Id = reader.IsDBNull(7) ? 0 : reader.GetUInt32(7);

                            }



                        }

                        return a;
                    }

                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);

                }
            }

            
        }

        public List<Appointment> GetPatientAppointments(uint patientId)
        {
            List<Appointment> aList = new List<Appointment>();

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                try
                {
                    connection.Open();
                    using (var command = new MySqlCommand(getPatientAppointments, connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@patient", patientId);

                        using var reader = command.ExecuteReader();
                        if (!reader.HasRows)
                        {
                            return aList;
                        }
                        while (reader.Read())
                        {
                            Appointment a = new Appointment();
                            a.Id = reader.GetUInt32(4);
                            a.Patient.Id = reader.GetUInt32(0);
                            a.Doctor.Id = reader.GetUInt32(1);
                            a.TimeStart = reader.GetDateTime(2);
                            a.RoomNumber = reader.GetUInt32(3);
                            a.State = (AppointmentState)reader.GetInt32(5);
                            a.ReasonForAppeal = reader.GetString(6);
                            a.Payment.Id = reader.IsDBNull(7) ? 0 : reader.GetUInt32(7);

                            //a.Payment.Id = reader.GetUInt32(7);
                            aList.Add(a);
                        }



                        //return a;
                    }
                    return aList;

                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);

                }
            }

          
        }
        public List<Appointment> GetAllPatientAppointments(uint patientId)
        {
            List<Appointment> aList = new List<Appointment>();

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                try
                {
                    connection.Open();
                    using (var command = new MySqlCommand(getAllPatientAppointments, connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@patient", patientId);

                        using var reader = command.ExecuteReader();
                        if (!reader.HasRows)
                        {
                            return aList;
                        }
                        while (reader.Read())
                        {
                            Appointment a = new Appointment();
                            a.Id = reader.GetUInt32(4);
                            a.Patient.Id = reader.GetUInt32(0);
                            a.Doctor.Id = reader.GetUInt32(1);
                            a.TimeStart = reader.GetDateTime(2);
                            a.RoomNumber = reader.GetUInt32(3);
                            a.State = (AppointmentState)reader.GetInt32(5);
                            a.ReasonForAppeal = reader.GetString(6);
                            a.Payment.Id = reader.IsDBNull(7) ? 0 : reader.GetUInt32(7);

                            //a.Payment.Id = reader.GetUInt32(7);
                            aList.Add(a);
                        }



                        //return a;
                    }
                    return aList;

                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);

                }
            }


        }

        public List<Appointment> GetDoctorAppointments(uint doctor)
        {
            List<Appointment> aList = new List<Appointment>();

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                try
                {
                    connection.Open();
                    using (var command = new MySqlCommand(getDoctorAppointments, connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@doctor", doctor);

                        using var reader = command.ExecuteReader();
                        
                            if (!reader.HasRows)
                            {
                                throw new NoSuchRecord("Ви не маєте жодного запису на прийом");
                            }
                            while (reader.Read())
                            {
                                Appointment a = new Appointment();
                                a.Id = reader.GetUInt32(4);
                                a.Patient.Id = reader.GetUInt32(0);
                                a.Doctor.Id = reader.GetUInt32(1);
                                a.TimeStart = reader.GetDateTime(2);
                                a.RoomNumber = reader.GetUInt32(3);
                                a.State = (AppointmentState)reader.GetInt32(5);
                                a.ReasonForAppeal = reader.GetString(6);
                                a.Payment.Id = a.Payment.Id = reader.IsDBNull(7) ? 0 : reader.GetUInt32(7);

                                //a.Payment.Id = reader.GetUInt32(7);
                                aList.Add(a);
                            }

                        


                        //return a;
                    }


                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);

                }
            }

            return aList;
        }
        
    }
}

