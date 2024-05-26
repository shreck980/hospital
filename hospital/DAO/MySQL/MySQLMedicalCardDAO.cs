using hospital.Entities;
using hospital.Exceptions;
using MySqlConnector;
using System.Data;

namespace hospital.DAO.MySQL
{
    public class MySQLMedicalCardDAO : LastIdGetter, IMedicalCardDAO
    {
        DAOConfig config;
        // private static string url =
        private const string InsertMedicalCard = "INSERT INTO medical_card (id, patient) VALUES (@id,@patient)";

        private const string InsertReferral = "INSERT INTO referral (id, doctor, expiration_date) VALUE (@id, @doctor, @expiration_date); ";

        private const string InsertEHR = "INSERT INTO ehr (id, appointment, result_of_examination) VALUES (@id, @appointment, @result_of_examination)";
        private const string InsertEMR = "INSERT INTO emr (referral, appointment, id) VALUES (@referral, @appointment, @id)";

        private const string InsertEHRMedicalCard = "INSERT INTO ehr_medical_card (ehr, medical_card) VALUES (@ehr, @medical_card)";
        private const string InsertEMRMedicalCard = "INSERT INTO emr_medical_card (emr, medical_card) VALUES (@emr, @medical_card)";

        private const string InsertEHRDrug = "INSERT INTO EHR_drug (drug, ehr_record, expiration_date) VALUES (@drug, @ehr_record, @expiration_date)";
        private const string InsertEHRSymptom = "INSERT INTO EHR_symptom (symptom, ehr_record) VALUES (@symptom, @ehr_record)";

        private const string GetMaxEMRId = "SELECT MAX(id) FROM emr";
        private const string GetMaxEHRId = "SELECT MAX(id) FROM ehr";
        private const string GetMaxReferralId = "SELECT MAX(id) FROM referral";

        private const string GetMedicalCardByPatietnId = "select * from medical_card where patient =@patient ;";
        private const string UpdateAppointmentState = "update appointment set state = @state where id = @appointment_id";
        public MySQLMedicalCardDAO(DAOConfig config)
        {
            this.config = config;
            GetLastID = "SELECT MAX(id) FROM medical_card";
        }

        public void AddMedicalCard(uint patientId)
        {

            MedicalCard m = new MedicalCard();

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        m.Id = GetLastId(connection, transaction) + 1;
                        using (var command = new MySqlCommand(InsertMedicalCard, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddWithValue("@id", m.Id);
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


        public void AddAppointmentRecord(MedicalCard card)
        {

            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var lastEHR = card.AppointmentRecord.Last().Value.Item2;
                        var lastEMR = card.AppointmentRecord.Last().Value.Item1;
                        var lastA = card.AppointmentRecord.Last().Key;

                        lastEHR.Id = GetLastId(connection, transaction, GetMaxEHRId) + 1;
                        lastEMR.Id = GetLastId(connection, transaction, GetMaxEMRId) + 1;



                        using (var command = new MySqlCommand(InsertEHR, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddWithValue("@id", lastEHR.Id);
                            command.Parameters.AddWithValue("@appointment", lastA.Id);
                            command.Parameters.AddWithValue("@result_of_examination", lastEHR.ResultOfExamination);


                            command.ExecuteNonQuery();
                        }
                        using (var command = new MySqlCommand(InsertEHRDrug, connection))
                        {
                            command.Transaction = transaction;
                            foreach (Drug drug in lastEHR.Drugs)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@drug", drug.Id);
                                command.Parameters.AddWithValue("@ehr_record", lastEHR.Id);
                                command.Parameters.AddWithValue("@expiration_date", drug.ExpirationDate);
                                command.ExecuteNonQuery();
                            }

                        }
                        using (var command = new MySqlCommand(InsertEHRSymptom, connection))
                        {
                            command.Transaction = transaction;
                            foreach (Symptom symptom in lastEHR.Symptoms)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@symptom", symptom.Id);
                                command.Parameters.AddWithValue("@ehr_record", lastEHR.Id);

                                command.ExecuteNonQuery();
                            }
                        }
                        if (lastEMR.Referral.Doctor.Id != 0)
                        {
                            lastEMR.Referral.Id = GetLastId(connection, transaction, GetMaxReferralId) + 1;
                            using (var command = new MySqlCommand(InsertReferral, connection))
                            {
                                command.Transaction = transaction;
                                command.Parameters.AddWithValue("@id", lastEMR.Referral.Id);
                                command.Parameters.AddWithValue("@doctor", lastEMR.Referral.Doctor.Id);
                                command.Parameters.AddWithValue("@expiration_date", lastEMR.Referral.ExpirationDate);
                                command.ExecuteNonQuery();

                            }
                        }
                        using (var command = new MySqlCommand(InsertEMR, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddWithValue("@referral", lastEMR.Referral.Id == 0 ? DBNull.Value : lastEMR.Referral.Id);
                            command.Parameters.AddWithValue("@appointment", lastA.Id);
                            command.Parameters.AddWithValue("@id", lastEMR.Id);
                            command.ExecuteNonQuery();

                        }
                        using (var command = new MySqlCommand(InsertEHRMedicalCard, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddWithValue("@medical_card", card.Id);
                            command.Parameters.AddWithValue("@ehr", lastEHR.Id);
                            command.ExecuteNonQuery();
                        }
                        using (var command = new MySqlCommand(InsertEMRMedicalCard, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddWithValue("@medical_card", card.Id);
                            command.Parameters.AddWithValue("@emr", lastEMR.Id);
                            command.ExecuteNonQuery();
                        }
                        using (var command = new MySqlCommand(UpdateAppointmentState, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddWithValue("@state", lastA.State);
                            command.Parameters.AddWithValue("@appointment_id", lastA.Id);
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

        public MedicalCard GetMedicalCardEmpty(uint patientId)
        {
            MedicalCard m = new MedicalCard();
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();

                try
                {

                    using (var command = new MySqlCommand(GetMedicalCardByPatietnId, connection))
                    {


                        command.Parameters.AddWithValue("@patient", patientId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                throw new NoSuchRecord();

                            }
                            while (reader.Read())
                            {

                                m.Id = reader.GetUInt32(0);



                            }
                        }
                    }


                    return m;
                }
                catch (MySQLException e)
                {

                    throw new MySQLException(e.Message, e);
                }

            }




        }

        public (EMR, EHR) GetAppointmentDetailsPatient(uint appointment)
        {
            using (MySqlConnection connection = new MySqlConnection(config.Url))
            {
                connection.Open();

                try
                {
                    EHR e = new EHR();
                    EMR e_m = new EMR();

                    using (var command = new MySqlCommand("Select *from ehr where appointment =@id;", connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@id", appointment);

                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                throw new NoSuchRecord();

                            }
                            while (reader.Read())
                            {

                                e.Id = reader.GetUInt32(0);
                                e.ResultOfExamination = reader.GetString(1);

                            }
                        }
                    }
                    using (var command = new MySqlCommand("Select e_d.*, d.* from ehr_drug e_d JOIN drug d ON e_d.drug = d.id where ehr_record =@id", connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@id", e.Id);

                        using (var reader = command.ExecuteReader())
                        {
                            Drug d = new Drug();
                            if (reader.HasRows)
                            {



                                while (reader.Read())
                                {

                                    d.Id = reader.GetUInt32(0);
                                    d.ExpirationDate = reader.GetDateTime(2);
                                    d.Name = reader.GetString(4);
                                    d.Instruction = reader.GetString(5);


                                }
                                e.Drugs.Add(d);
                            }
                        }
                    }


                   

                    using (var command = new MySqlCommand("select e.*, r.id as referral_id,r.doctor,r.expiration_date,d.Name,d.Surname from emr e join referral r on e.referral = r.id join doctor_account d on r.doctor=d.id where appointment = @appointment", connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@appointment", appointment);

                        using (var reader = command.ExecuteReader())
                        {

                            if (reader.HasRows)
                            {



                                while (reader.Read())
                                {

                                    e_m.Id = reader.GetUInt32(0);
                                    e_m.Referral.Id = reader.GetUInt32(1);
                                    e_m.Referral.Doctor.Id = reader.GetUInt32(3);
                                    e_m.Referral.Doctor.Name = reader.GetString(6);
                                    e_m.Referral.Doctor.Surname = reader.GetString(7);
                                    e_m.Referral.ExpirationDate = reader.GetDateTime(5);


                                }
                            }

                        }
                    }
                    return (e_m,e);
                }
                catch (MySQLException e)
                {

                    throw new MySQLException(e.Message, e);
                }

            }
                
        }
    

            public MedicalCard GetMedicalCardPatient(uint patientId)
            {
               MedicalCard m = new MedicalCard();
               using (MySqlConnection connection = new MySqlConnection(config.Url))
               {
                connection.Open();

                try
                {

                    using (var command = new MySqlCommand(GetMedicalCardByPatietnId, connection))
                    {

                        command.Parameters.AddWithValue("@patient", patientId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                throw new NoSuchRecord();

                            }
                            while (reader.Read())
                            {

                                m.Id = reader.GetUInt32(0);



                            }
                        }
                    }
                    List<Appointment> appointments = new List<Appointment>();
                   

                    using (var command = new MySqlCommand("SELECT a.*, d.Name,d.Surname FROM appointment a join doctor_account d on a.doctor = d.id where patient = @patient;", connection))
                    {
                        command.Parameters.AddWithValue("@patient", patientId);

                        using var reader = command.ExecuteReader();
                        if (!reader.HasRows)
                        {
                            throw new NoSuchRecord("Не можемо знайти інформацію про Ваші прийоми, будь ласка спробуйте пізніше");
                        }
                        while (reader.Read())
                        {
                            Appointment a = new Appointment();
                            a.Id = reader.GetUInt32(4);
                            a.Patient.Id = reader.GetUInt32(0);
                            a.Doctor.Id = reader.GetUInt32(1);
                            a.Doctor.Name = reader.GetString(8);
                            a.Doctor.Surname = reader.GetString(9);
                            a.TimeStart = reader.GetDateTime(2);
                            a.RoomNumber = reader.GetUInt32(3);
                            a.State = (AppointmentState)reader.GetInt32(5);
                            a.ReasonForAppeal = reader.GetString(6);
                            a.Payment.Id = reader.IsDBNull(7) ? 0 : reader.GetUInt32(7);

                            //a.Payment.Id = reader.GetUInt32(7);
                            appointments.Add(a);
                        }


                        
                    }
                    /*foreach (Appointment a in appointments)
                    {
                        EHR e = new EHR();
                       
                        using (var command = new MySqlCommand("Select *from ehr where appointment =@id;", connection))
                        {
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@id", a.Id);
                           
                            using (var reader = command.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    throw new NoSuchRecord();

                                }
                                while (reader.Read())
                                {

                                   e.Id = reader.GetUInt32(0);
                                   e.ResultOfExamination = reader.GetString(1);

                                }
                            }
                        }
                        using (var command = new MySqlCommand("Select e_d.*, d.* from ehr_drug e_d JOIN drug d ON e_d.drug = d.id where ehr_record =@id", connection))
                        {
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@id", e.Id);

                            using (var reader = command.ExecuteReader())
                            {
                                Drug d = new Drug();
                                if (reader.HasRows)
                                {



                                    while (reader.Read())
                                    {

                                        d.Id = reader.GetUInt32(0);
                                        d.ExpirationDate = reader.GetDateTime(2);
                                        d.Name = reader.GetString(4);
                                        d.Instruction = reader.GetString(5);


                                    }
                                    e.Drugs.Add(d);
                                }
                            }
                        }


                        EMR e_m = new EMR();
                       
                        using (var command = new MySqlCommand("select e.*, r.id as referral_id,r.doctor,r.expiration_date,d.Name,d.Surname from emr e join referral r on e.referral = r.id join doctor_account d on r.doctor=d.id where appointment = @appointment", connection))
                        {
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@appointment",a.Id);

                            using (var reader = command.ExecuteReader())
                            {

                                if (reader.HasRows)
                                {



                                    while (reader.Read())
                                    {

                                        e_m.Id = reader.GetUInt32(0);
                                        e_m.Referral.Id = reader.GetUInt32(1);
                                        e_m.Referral.Doctor.Id = reader.GetUInt32(3);
                                        e_m.Referral.Doctor.Name = reader.GetString(6);
                                        e_m.Referral.Doctor.Surname = reader.GetString(7);
                                        e_m.Referral.ExpirationDate = reader.GetDateTime(5);


                                    }
                                }
                              
                            }
                        }*/
                       // m.AppointmentRecord.Add(a, (e_m, e));

                    //}
                    foreach(Appointment a in appointments)
                    {
                        m.AppointmentRecord.Add(a,( null, null));
                    }

                    return m;
                }
                catch (MySQLException e)
                {

                    throw new MySQLException(e.Message, e);
                }

            }
        }

    }

}
