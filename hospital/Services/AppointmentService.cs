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

        public long AddAppointment(BookAppointment model, long? patientId)
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
                a.RoomNumber = (long)random.Next(1, 50);
                a.Payment = null;
                if (model.ReferralId.HasValue)
                {
                    a.State = AppointmentState.PlannedByReferral;
                }
                else
                {
                    a.State = AppointmentState.Planned;
                }
               
                
                _appointmentDAO.AddApppointment(a);
                _scheduleDAO.MarkEventAsBooked(s.Id, a.Id);
                return a.Id;
               
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
        }

        public List<Appointment> GetPatientAppointmentList(long id)
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

        public List<Appointment> GetAllPatientAppointmentList(long id)
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
        public Appointment GetAppointmentById(long id)
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

        public Appointment GetAppointmentByIdAllStates(long id)
        {
            try
            {
                Appointment a = _appointmentDAO.GetAppointmentByIdAllStates(id);
                a.Doctor = _doctorDAO.GetDoctorById(a.Doctor.Id);
                return a;

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }

        }
        public void AddPaymentToAppointment(Payment p, long appointmentId)
        {
            try
            {
                _appointmentDAO.PaymentToApppointment(p, appointmentId);
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
        }

       

        public void CancelAppointment(long id) {

            try
            {
                _appointmentDAO.CancelAppointment(id);
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
        }
        public void UpdateState(Appointment a)
        {
            try
            {
                _appointmentDAO.UpdateState(a);

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
            
        }
    }
}
