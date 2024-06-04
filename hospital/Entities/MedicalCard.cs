namespace hospital.Entities
{
   
    public class MedicalCard
    {
        public long Id { get; set; }
        
        public Dictionary<Appointment, (EMR?, EHR?)> AppointmentRecord;
        public MedicalCard()
        {
            //Id = GenerateId();
            Id = 0;
            AppointmentRecord = new Dictionary<Appointment, (EMR?, EHR?)>();
        }
       
    }
}
