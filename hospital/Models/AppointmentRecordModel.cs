using hospital.Entities;


namespace hospital.Models
{
    public class AppointmentRecordModel
    {
        public Appointment Appointment { get; set; }
        public EHR EHR { get; set; }
        public EMR EMR { get; set; }
       
        public AppointmentRecordModel()
        {

            Appointment = new Appointment();
            EHR = new EHR();
            EMR = new EMR();
        }
    }
}
