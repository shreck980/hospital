using hospital.Entities;

namespace hospital.DAO
{
    public interface IScheduleDAO
    {
        public Event GetEvenById(long id);
        public List<Event> GetScheduleByDoctorIdForPatient(long doctor);
        public List<Event> GetScheduleByDoctorIdForDoctor(long doctor);
        public void AddSchedule(List<Event> schedule, long doctor);
        public void MarkEventAsBooked(long eventId, long appointmentId);
    }
}
