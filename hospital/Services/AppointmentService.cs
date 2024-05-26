using hospital.Models;
using hospital.Entities;
using System.Transactions;
using hospital.DAO;
using hospital.Exceptions;

namespace hospital.Services
{
    public class AppointmentService
    {

        IDoctorDAO _doctorDAO;
        IScheduleDAO _scheduleDAO;
        IPatientDAO _patientDAO;
        IAppointmentDAO _appointmentDAO;

        public AppointmentService(IDoctorDAO doctorDAO, IScheduleDAO scheduleDAO, IPatientDAO patientDAO, IAppointmentDAO appointmentDAO)
        {

            _doctorDAO = doctorDAO;
            _scheduleDAO = scheduleDAO;
            _patientDAO = patientDAO;
            _appointmentDAO = appointmentDAO;
        }

        public void AddAppointment(BookAppointment model, uint? patientId)
        {
            try
            {
                Appointment a = new Appointment();
                a.Doctor = _doctorDAO.GetDoctorById(model.Doctor.Id);
              
                a.Patient = _patientDAO.GetPatientById(patientId.HasValue ? patientId.Value : 0);
                Event s = _scheduleDAO.GetEvenById(model.ScheduleId.HasValue ? model.ScheduleId.Value : 0);
                if (s.Id == 0)
                {
                    throw new MySQLException("Такого часу для прийому нема, будь ласка оберіть ще раз");
                }
                a.TimeStart = s.Start;
              
                a.ReasonForAppeal = model.ReasonForAppeal == null ? "" : model.ReasonForAppeal;
                Random random = new Random();
                a.RoomNumber = (uint)random.Next(1, 50);
                if(a.Doctor.Speciality==Speciality.Dentist || a.Doctor.Speciality == Speciality.Ophthalmologist || a.Doctor.Speciality == Speciality.Gynecologist)
                {
                    a.Payment = new Payment();
                    a.Payment.Patient = a.Patient;
                    a.Payment.DateIssued = a.TimeStart;
                }
                else
                {
                    a.Payment = null;
                }
                
                a.State = AppointmentState.Planned;
               
                
                _appointmentDAO.AddApppointment(a);
                _scheduleDAO.MarkEventAsBooked(s.Id, a.Id);
               
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
        }

        public List<Appointment> GetPatientAppointmentList(uint id)
        {
            try
            {
                List<Appointment> appointments = _appointmentDAO.GetPatientAppointments(id);
                foreach(Appointment a in appointments)
                {
                    a.Doctor = _doctorDAO.GetDoctorById(a.Doctor.Id);
                }
                return appointments;
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
           
        }

        public List<Appointment> GetAllPatientAppointmentList(uint id)
        {
            try
            {
                List<Appointment> appointments = _appointmentDAO.GetAllPatientAppointments(id);
                foreach (Appointment a in appointments)
                {
                    a.Doctor = _doctorDAO.GetDoctorById(a.Doctor.Id);
                }
                return appointments;
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }

        }
        public Appointment GetAppointmentById(uint id)
        { 
            try
            {
                Appointment a = _appointmentDAO.GetAppointmentById(id); 
                return a;
               
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }

        }

        public void ChangeAppointmentState(uint id)
        {

        }
    }
}
