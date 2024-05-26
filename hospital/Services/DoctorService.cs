using hospital.DAO;
using hospital.Exceptions;
using hospital.Entities;


namespace hospital.Services
{
    public class DoctorService
    {
        IDoctorDAO _doctorDAO;
        IScheduleDAO _scheduleDAO;

        public DoctorService(IDoctorDAO doctorDAO, IScheduleDAO scheduleDAO)
        {

            _doctorDAO = doctorDAO;
            _scheduleDAO = scheduleDAO;
        }

       public List<Doctor> GetDoctorsBySpeciality(Speciality speciality)
       {
            return _doctorDAO.GetDoctorsBySpeciality(speciality);
       }
        public Doctor GetDoctorsByEmail(string email)
        {
            try
            {
                Doctor d = _doctorDAO.GetDoctorByEmail(email);
                return d;
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

        public Doctor GetDoctorsById(uint id)
        {
            try
            {
                Doctor d = _doctorDAO.GetDoctorById(id);
                return d;
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
        public Doctor GetDoctorsByIdMin(uint id)
        {
            try
            {
                Doctor d = _doctorDAO.GetDoctorByIdMin(id);
                return d;
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

        public List<Doctor> GetAllDoctors()
        {
            try
            {
               return  _doctorDAO.GetAllDoctorsExceptTherapist();
              
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


        public string GetSpeciality(Speciality s)
        {
            try
            {
                return _doctorDAO.GetSpeciality(s);

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

        public List<Doctor> GetAllDoctorsForPatient()
        {
            try
            {
                return _doctorDAO.GetAllDoctorsForPatient();

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
