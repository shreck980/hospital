using hospital.Entities;

namespace hospital.DAO
{
    public interface IAppointmentDAO
    {
        public void AddApppointment(Appointment a);
        public void CancelAppointment(long id);
        public void PaymentToApppointment(Payment p, long appointmentId);
        public Appointment? GetAppointmentByPatientAndTime(Patient p, DateTime time);
        public List<Appointment> GetPatientAppointments(long patientId);
        public List<Appointment> GetAllPatientAppointments(long patientId);
        public List<Appointment> GetDoctorAppointments(long doctor);
        public Appointment GetAppointmentById(long id);
        public Appointment GetAppointmentByIdAllStates(long id);
        public void UpdateState(Appointment a);
    }
}
