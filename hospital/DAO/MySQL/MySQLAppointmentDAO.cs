using MySqlConnector;
using System.Data;
using hospital.Entities;
using hospital.Exceptions;
using System.Transactions;
using System.Collections.Generic;
using System.Data.Common;

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
        private const string getAppointmentById = "SELECT* FROM appointment where id = @id and (state in(1,2) or state=5);";
        private const string getAppointmentByIdAllStates = "SELECT* FROM appointment where id = @id;";
        private const string getDoctorAppointments = "SELECT* FROM appointment where doctor = @doctor;";
        private const string GetLastMaxIdPayment = "SELECT MAX(id) FROM payment_info;";


        /*  public  long GetLastAppointmentdId(MySqlConnection connection, MySqlTransaction transaction)
          {
              long id = 1;



              using (MySqlCommand command = new MySqlCommand(GetLastID, connection))
              {
                  command.Transaction = transaction;
                  var result = command.ExecuteScalar();
                  if (result != null && result != DBNull.Value)
                  {
                      id = Convert.ToInt64(result);
                  }
                  else
                  {
                      throw new MySQLException("Error retrieving Appointment information");


                  }
              }
              return id;




          }*/

        /* public  long GetLastPaymentdId(MySqlConnection connection, MySqlTransaction transaction)
         {
             long id = 1;
             using (MySqlCommand command = new MySqlCommand(GetLastMaxIdPayment, connection))
             {
                 command.Transaction = transaction;
                 var result = command.ExecuteScalar();
                 if (result != null && result != DBNull.Value)
                 {
                     id = Convert.ToInt64(result);
                 }
                 else
                 {
                     throw new MySQLException("Error retrieving Payment information");


                 }
             }
             return id;




         }*/
        public void UpdateState(Appointment a)
        {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        using (var command = new MySqlCommand("update appointment set state=@state where id=@id;", connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddWithValue("@state", a.State);
                            command.Parameters.AddWithValue("@id", a.Id);
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

            public void AddApppointment(Appointment a)
            {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {

                    try
                    {
                       
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


        public void PaymentToApppointment(Payment p, long appointmentId)
        {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {

                    try
                    {
                        p.Id = GetLastId(connection, transaction, GetLastMaxIdPayment) + 1;
                        using (MySqlCommand cmd = new MySqlCommand(InsertPayment, connection))
                        {
                            cmd.Transaction = transaction;

                            cmd.Parameters.AddWithValue("@id", p.Id);
                            cmd.Parameters.AddWithValue("@price", p.Price);
                            cmd.Parameters.AddWithValue("@date_issued", p.DateIssued);
                            cmd.Parameters.AddWithValue("@date_paid", p.DatePaid == null ? DBNull.Value : p.DatePaid);
                            cmd.Parameters.AddWithValue("@patient", p.Patient.Id);

                            cmd.ExecuteNonQuery();
                        }

                        using (MySqlCommand cmd = new MySqlCommand("update appointment set payment=@id where id=@aId;", connection))
                        {
                            cmd.Transaction = transaction;
                            cmd.Parameters.AddWithValue("@id", p.Id);
                            cmd.Parameters.AddWithValue("@aId", appointmentId);

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

        public void CancelAppointment(long id)
        {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                       /* using (MySqlCommand cmd = new MySqlCommand("DELETE FROM schedule WHERE appointment = @id", connection, transaction))
                        {

                            cmd.Parameters.AddWithValue("@id", id);


                            cmd.ExecuteNonQuery();
                        }*/
                        using (MySqlCommand cmd = new MySqlCommand("DELETE FROM appointment WHERE id = @id", connection, transaction))
                        {
                            
                            cmd.Parameters.AddWithValue("@id", id);

                          
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (MySQLException e)
                    {
                        transaction.Rollback();
                        throw new MySQLException("Error while cancelling appointment: " + e.Message, e);
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
                                a = MapAppointment(reader);
                               

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

        public Appointment GetAppointmentById(long id)
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
                                throw new NoSuchRecord("Помилка знайти прийом ");
                            }

                            while (reader.Read())
                            {

                                a = MapAppointment(reader);

                            }



                        }

                       
                    }
                    return a;

                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);

                }
            }


        }

        public Appointment GetAppointmentByIdAllStates(long id)
        {


            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                Appointment a = new Appointment();
                try
                {
                    connection.Open();
                    using (var command = new MySqlCommand(getAppointmentByIdAllStates, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        //command.Parameters.AddWithValue("@time", time.ToString("yyyy-MM-dd HH:mm:ss"));
                        using (var reader = command.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new NoSuchRecord("Помилка знайти прийом ");
                            }

                            while (reader.Read())
                            {

                                a = MapAppointment(reader);

                            }



                        }
                    }

                        if (a.Payment.Id != 0)
                        {
                            using (var command = new MySqlCommand("select*from payment_info where id=@id", connection))
                            {
                               
                                command.Parameters.AddWithValue("@id", a.Payment.Id);

                                using (var reader = command.ExecuteReader())
                                {
                                    if (!reader.HasRows)
                                    {
                                        throw new NoSuchRecord("Помилка знайти прийом ");
                                    }
                                    while (reader.Read())
                                    {

                                        a.Payment.Price = reader.GetDecimal(1);
                                        a.Payment.DateIssued = reader.GetDateTime(2);
                                        a.Payment.DatePaid = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3);
                                        a.Payment.Patient.Id = reader.GetInt64(4);
                                        if (a.Payment.Patient.Id == a.Patient.Id)
                                        {
                                            a.Payment.Patient = a.Patient;
                                        }

                                    }
                                }

                            }
                        }

                        return a;
                    

                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);

                }
            }


        }

        public List<Appointment> GetPatientAppointments(long patientId)
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
                            

                            //a.Payment.Id = reader.GetInt64(7);
                            aList.Add(MapAppointment(reader));
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
        public List<Appointment> GetAllPatientAppointments(long patientId)
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
                           

                            //a.Payment.Id = reader.GetInt64(7);
                            aList.Add(MapAppointment(reader));
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

        public List<Appointment> GetDoctorAppointments(long doctor)
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
                            aList.Add(MapAppointment(reader));
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

        private Appointment MapAppointment(MySqlDataReader reader)
        {
            try
            {
                Appointment a = new Appointment();
                a.Id = reader.GetInt64(4);
                a.Patient.Id = reader.GetInt64(0);
                a.Doctor.Id = reader.GetInt64(1);
                a.TimeStart = reader.GetDateTime(2);
                a.RoomNumber = reader.GetInt64(3);
                a.State = (AppointmentState)reader.GetInt32(5);
                a.ReasonForAppeal = reader.GetString(6);
                a.Payment.Id = reader.IsDBNull(7) ? 0 : reader.GetInt64(7);
                return a;
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);

            }
        }



       

    }
}


