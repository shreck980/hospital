using hospital.DAO.MySQL;
using hospital.DAO;
using hospital.Models;
using System.Reflection;
using hospital.Exceptions;
using hospital.Entities;
using System.Linq.Expressions;

namespace hospital.Services

{
    public class ScheduleService
    {

        private readonly IScheduleDAO _scheduleDAO;
        private readonly IDoctorDAO _doctorDAO;
        public ScheduleService(IScheduleDAO scheduleDAO, IDoctorDAO doctorDAO)
        {
            _scheduleDAO = scheduleDAO;
            _doctorDAO = doctorDAO;
        }

        public bool RetrieveScheduleForPatientView(BookAppointment model)
        {
            try
            {
                model.Doctor = _doctorDAO.GetDoctorById(model.Doctor.Id);
                var schedule = _scheduleDAO.GetScheduleByDoctorIdForPatient(model.Doctor.Id);
                if (schedule.Count > 0)
                {
                    model.Doctor.Schedule = schedule;
                    model.ScheduleForView();
                    return true;
                }
                return false;
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

        public List<Event> RetrieveScheduleForDoctorView(uint doctorId)
        {
            try
            {

                List<Event> schedule = _scheduleDAO.GetScheduleByDoctorIdForDoctor(doctorId);
                return schedule;
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
