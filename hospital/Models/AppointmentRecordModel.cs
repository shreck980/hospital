using hospital.Entities;
using System.ComponentModel.DataAnnotations;


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
            HasPayment = false;
            Attended = false;
        }
        [Display(Name ="Платна послуга:")]
        public bool HasPayment {  get; set; }
        [Display(Name = "Прийом відвідано:")]
        public bool Attended {  get; set; }
    }
}
