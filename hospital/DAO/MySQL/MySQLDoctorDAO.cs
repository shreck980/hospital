using hospital.Entities;
using hospital.Exceptions;
using MySqlConnector;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Transactions;
using System.Xml.Linq;

namespace hospital.DAO.MySQL
{
    public class MySQLDoctorDAO : LastIdGetter, IDoctorDAO
    {
        DAOConfig config;
        private const string InsertDoctor = "INSERT INTO doctor_account (id, name, surname, email, password, state, inaccessibility, speciality) VALUES (@id, @Name, @Surname, @Email, @Password, @State,@inaccessibility,@Speciality)";


        public MySQLDoctorDAO(DAOConfig config)
        {
            this.config = config;
            GetLastID = "SELECT MAX(id) FROM doctor_account";
        }

        /*private long GetLastDoctortId(MySqlConnection connection,MySqlTransaction transaction) {
            long id = 1;
            string query = "SELECT MAX(id) FROM doctor_account";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Transaction = transaction;
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    id = Convert.ToInt64(result);
                }
                else
                {
                    throw new MySQLException("Error retrieving Doctor information");
                

                }
            }

            return id;
        }*/
        public void AddDoctor(Doctor d)
        {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        d.Id = GetLastId(connection, transaction) + 1;

                        using (var command = new MySqlCommand(InsertDoctor, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddWithValue("@Name", d.Name);
                            command.Parameters.AddWithValue("@Id", d.Id);
                            command.Parameters.AddWithValue("@Surname", d.Surname);
                            command.Parameters.AddWithValue("@Email", d.Email);
                            command.Parameters.AddWithValue("@Password", d.Password);
                            command.Parameters.AddWithValue("@State", d.State);
                            command.Parameters.AddWithValue("@Inaccessibility", DBNull.Value);
                            command.Parameters.AddWithValue("@Speciality", d.Speciality);


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
        public Doctor GetDoctorById(long id)
        {

            // using var connection = new MySqlConnection(config.Url);
            Doctor d = new Doctor();
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {

                using var command = new MySqlCommand("SELECT* FROM doctor_account where id = @id", connection);
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {

                        if (!reader.HasRows)
                        {
                            throw new NoSuchRecord("Помилка при пошуку доктора, будь ласка спробуйте ще раз або пізніше");
                        }
                        while (reader.Read())
                        {
                            d=MapDoctorFull(reader);
                            Console.WriteLine(d);

                        }
                    }
                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);
                }
            }

            return d;


        }

        public Doctor GetDoctorByIdMin(long id)
        {
            Doctor d = new Doctor();
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {

                using var command = new MySqlCommand("SELECT* FROM doctor_account where id = @id", connection);
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {

                        if (!reader.HasRows)
                        {
                            throw new NoSuchRecord("Помилка при пошуку доктора, будь ласка спробуйте ще раз або пізніше");
                        }
                        while (reader.Read())
                        {
                            d.Id = reader.GetInt64(0);
                            d.Name = reader.GetString(1);
                            d.Surname = reader.GetString(2);
                            d.Speciality = (Speciality)reader.GetInt32(7);
                            Console.WriteLine(d);

                        }
                    }
                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);
                }
            }

            return d;
        }
        public Doctor GetDoctorByEmail(string email)
        {

            // using var connection = new MySqlConnection(config.Url);
            Doctor d = new Doctor();
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {

                using var command = new MySqlCommand("SELECT* FROM doctor_account where email = @email and state =@state ", connection);
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@state", AccountStates.Active);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            throw new NoSuchRecord("Акаунту з такою поштою не існує");
                        }
                        while (reader.Read())
                        {
                            d = MapDoctorFull(reader);
                            Console.WriteLine(d);

                        }
                    }
                    return d;

                }
                catch (MySQLException e)
                {
                    throw new MySQLException(e.Message, e);
                }
            }




        }

        public List<Doctor> GetDoctorsBySpeciality(Speciality speciality)
        {
            string query = "SELECT* FROM doctor_account where speciality = @speciality and state =@state";
            if (speciality == Speciality.Therapist)
            {
                query = "select*from doctor_account d where speciality = @speciality and state =@state  and (select count(id) from patient_account where family_doctor=d.id)<2000;";
            }
            // using var connection = new MySqlConnection(config.Url);
            List<Doctor> doctors = new List<Doctor>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(config.Url))
                {
                    using (var command = new MySqlCommand(query, connection))
                    {

                        connection.Open();
                        command.Parameters.AddWithValue("@speciality", speciality);
                        command.Parameters.AddWithValue("@state", AccountStates.Active);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                throw new NoSuchRecord("Помилка при пошуку лікарів, будь ласка спробуйте пізніше");
                            }
                            while (reader.Read())
                            {
                                Doctor d = new Doctor();
                                d.Id = reader.GetInt64(0);
                                d.Name = reader.GetString(1);
                                d.Surname = reader.GetString(2);
                                //d.Email = reader.GetString(3);
                                //d.Password = reader.GetString(4);
                                d.State = (AccountStates)reader.GetInt16(5);
                                d.Speciality = (Speciality)reader.GetInt16(7);
                                Console.WriteLine(d);

                                doctors.Add(d);
                            }
                        }
                    }

                }
                return doctors;

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }





        }


        public List<Doctor> GetAllDoctorsExceptTherapist()
        {

            // using var connection = new MySqlConnection(config.Url);
            List<Doctor> doctors = new List<Doctor>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(config.Url))
                {
                    using (var command = new MySqlCommand("SELECT* FROM doctor_account where speciality != 2 and state =@state", connection))
                    {

                        connection.Open();

                        command.Parameters.AddWithValue("@state", AccountStates.Active);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                throw new NoSuchRecord("Помилка при пошуку лікарів, будь ласка спробуйте пізніше");
                            }
                            while (reader.Read())
                            {
                                Doctor d = new Doctor();
                                d.Id = reader.GetInt64(0);
                                d.Name = reader.GetString(1);
                                d.Surname = reader.GetString(2);
                                //d.Email = reader.GetString(3);
                                //d.Password = reader.GetString(4);
                                d.State = (AccountStates)reader.GetInt16(5);
                                d.Speciality = (Speciality)reader.GetInt16(7);
                                Console.WriteLine(d);

                                doctors.Add(d);
                            }
                        }
                    }

                }
                return doctors;

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
        }

        public List<Doctor> GetAllDoctorsForPatient()
        {

            // using var connection = new MySqlConnection(config.Url);
            List<Doctor> doctors = new List<Doctor>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(config.Url))
                {
                    using (var command = new MySqlCommand("SELECT* FROM doctor_account where speciality = 1 or speciality = 13 or speciality = 5 and state =@state", connection))
                    {

                        connection.Open();

                        command.Parameters.AddWithValue("@state", AccountStates.Active);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                throw new NoSuchRecord("Помилка при пошуку лікарів, будь ласка спробуйте пізніше");
                            }
                            while (reader.Read())
                            {
                                Doctor d = new Doctor();
                                d.Id = reader.GetInt64(0);
                                d.Name = reader.GetString(1);
                                d.Surname = reader.GetString(2);
                                //d.Email = reader.GetString(3);
                                //d.Password = reader.GetString(4);
                                //d.State = (AccountStates)reader.GetInt16(5);
                                d.Speciality = (Speciality)reader.GetInt16(7);
                                Console.WriteLine(d);

                                doctors.Add(d);
                            }
                        }
                    }

                }
                return doctors;

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
        }
        public string GetSpeciality(Speciality speciality)
        {


            try
            {
                string s = "";
                using (MySqlConnection connection = new MySqlConnection(config.Url))
                {

                    using var command = new MySqlCommand("SELECT* FROM speciality where id = @id", connection);

                    connection.Open();

                    command.Parameters.AddWithValue("@id", speciality);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            throw new NoSuchRecord("Акаунту з такою поштою не існує");
                        }
                        while (reader.Read())
                        {
                            s = reader.GetString(1);

                        }
                    }
                }
                return s;
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
        }


        public Doctor MapDoctorFull(MySqlDataReader reader)
        {
            try
            {
                Doctor d = new Doctor();
                d.Id = reader.GetInt64(0);
                d.Name = reader.GetString(1);
                d.Surname = reader.GetString(2);
                d.Email = reader.GetString(3);
                d.Password = reader.GetString(4);
                d.State = (AccountStates)reader.GetInt16(5);
                d.Speciality = (Speciality)reader.GetInt16(7);
                return d;
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
        }
    }
}





