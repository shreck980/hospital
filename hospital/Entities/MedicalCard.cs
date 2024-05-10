namespace hospital.Entities
{
    public class MedicalCard
    {
        public uint Id { get; set; }
        private static uint lastId;
        public Dictionary<Appointment, (EMR, EHR)> AppointmentRecord;
        public MedicalCard()
        {
            Id = GenerateId();
            AppointmentRecord = new Dictionary<Appointment, (EMR, EHR)>();
        }
        private static uint GenerateId()
        {
            return ++lastId;
        }
    }
}
