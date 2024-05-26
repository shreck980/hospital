namespace hospital.Entities
{
    public class Event
    {

        public uint Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Appointment? Appointment { get; set; }

        public Event(DateTime start, DateTime end,Appointment? a)
        {
            Start = start;
            End = end;
            Appointment = a;
        }

        public Event()
        {
            Start = new DateTime();
            End = new DateTime();
            Appointment = null;
        }
    }
}
