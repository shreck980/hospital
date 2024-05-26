using hospital.DAO;
using hospital.Entities;
using hospital.Exceptions;

namespace hospital.Services
{
    public class MedicalCardService
    {
        IMedicalCardDAO _medicalCardDAO;
        public MedicalCardService(IMedicalCardDAO medicalCardDAO) {
            _medicalCardDAO = medicalCardDAO;
        }
        public MedicalCard GetMedicalCardEmpty(uint id)
        {
            try
            {
                MedicalCard a = _medicalCardDAO.GetMedicalCardEmpty(id);
                return a;

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
        public MedicalCard GetMedicalCardForPatient(uint id)
        {
            try
            {
                MedicalCard a = _medicalCardDAO.GetMedicalCardPatient(id);
                return a;

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
        public void AddMedicalCard(uint patientId)
        {
            try
            {
                _medicalCardDAO.AddMedicalCard(patientId);

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
           
        }
        public void AddAppointmentRecord(MedicalCard medicalCard)
        {
            try
            {
                _medicalCardDAO.AddAppointmentRecord(medicalCard);

            }
            catch (NoSuchRecord e)
            {
                throw new NoSuchRecord(e.Message, e);
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }

        }

        public (EMR, EHR) GetAppointmentDetailsPatient(uint appointment)
        {
            try
            {
                return _medicalCardDAO.GetAppointmentDetailsPatient(appointment);
                
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

    }
}
