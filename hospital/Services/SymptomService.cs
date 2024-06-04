using hospital.DAO;
using hospital.Entities;
using hospital.Exceptions;

namespace hospital.Services
{
    public class SymptomService
    {
        ISymptomDAO _symptomDAO;

        public SymptomService(ISymptomDAO symptomDAO)
        {
            _symptomDAO = symptomDAO;
        }

        public List<Symptom> GetAllSymptoms()
        {
            try
            {
                return _symptomDAO.GetAllSymptoms();
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

        public List<Symptom> GetAllSymptomsPerAppointment(long ehr)
        {
            try
            {
                return _symptomDAO.GetAllSymptomsPerAppointment(ehr);
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
