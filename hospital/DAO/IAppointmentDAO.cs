using hospital.Entities;

namespace hospital.DAO
{
    public interface IAppointmentDAO
    {
        public void AddApppointment(Appointment a);
        public Appointment? GetAppointmentByPatientAndTime(Patient p, DateTime time);
        public List<Appointment> GetPatientAppointments(uint patientId);
        public List<Appointment> GetAllPatientAppointments(uint patientId);
        public List<Appointment> GetDoctorAppointments(uint doctor);
        public Appointment GetAppointmentById(uint id);
    }
}
