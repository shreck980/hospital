using hospital.DAO;
using hospital.DAO.MySQL;
using hospital.Entities;
using System.Reflection;
using hospital.Exceptions;



namespace hospital.Services
{
    public class PatientService
    {
        IPatientDAO _patientDAO;
        
        

        public PatientService(IPatientDAO patientDAO)
        {
            _patientDAO = patientDAO;
          
            
        }
        public Patient GetPatientByID(long id)
        {
            try
            {
                Patient p = _patientDAO.GetPatientById(id);
                return p;
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
            catch (NoSuchRecord e)
            {
                throw new NoSuchRecord(e.Message, e);
            }

        }

        public Patient GetPatientByIDSecure(long id)
        {
            try
            {
                Patient p = _patientDAO.GetPatientByIdSecure(id);
                return p;
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
            catch (NoSuchRecord e)
            {
                throw new NoSuchRecord(e.Message, e);
            }

        }
        public Patient GetPatientByEmail(string email)
        {
            try
            {
               Patient p= _patientDAO.GetPatientByEmail(email);
                return p;
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
            catch (NoSuchRecord e)
            {
                throw new NoSuchRecord(e.Message, e);
            }
        }

        public void VerificateAccount(string email)
        {
            Patient? patient = _patientDAO.GetPatientByEmailUnverified(email);
            if (patient == null)
            {
                throw new MySQLException("Ваш обліковий запис не знайдено або ви вже зареєстровані");

            }
            patient.State = AccountStates.Verified;
            _patientDAO.UpdatePatientWithСonfirmation(patient);
           
        }

        public void CreateUnverifiedAccount(Patient model)
        {
            model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            _patientDAO.AddPatientWithoutСonfirmation(model);
            EmailService emailService = new EmailService();
            emailService.SendVerificationEmail(model.Email);
        }

        public Patient GetPatientMinById(long id)
        {
            try
            {
               Patient p = _patientDAO.GetPatientMinDataById(id);
               return p;
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
            catch (NoSuchRecord e)
            {
                throw new NoSuchRecord(e.Message, e);
            }
        }

        public void SetFamilyDoctor(Patient p)
        {

            try
            {
                _patientDAO.UpdatePatientFamilyDoctor(p);
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
           

        }
        public void UpdateState(AccountStates state,long id)
        {
            try
            {
                _patientDAO.SetAccountState(state, id);
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
            
        }


    }
}
