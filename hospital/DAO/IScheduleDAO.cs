using hospital.Entities;

namespace hospital.DAO
{
    public interface IScheduleDAO
    {
        public Event GetEvenById(uint id);
        public List<Event> GetScheduleByDoctorIdForPatient(uint doctor);
        public List<Event> GetScheduleByDoctorIdForDoctor(uint doctor);
        public void AddSchedule(List<Event> schedule, uint doctor);
        public void MarkEventAsBooked(uint eventId, uint appointmentId);
    }
}
