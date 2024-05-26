using System.Xml.Linq;

namespace hospital.Entities
{
    
    public class Doctor:Account
    {
       
      
        public string Name { get; set; }
        public string Surname { get; set; }

        public AccountStates State { get; set; }
        public Speciality Speciality { get; set; }

        public List<Event> Schedule {  get; set; }

        public Doctor(uint id, string name, string surname, string email, string password, AccountStates state, List<Event> schedule, Speciality speciality)
            : base(id, email, password)
        {
           
            Name = name;
            Surname = surname;
            this.State = state;
            Schedule = schedule;
            Speciality = speciality;
        }

        public Doctor(string name, string surname, string email, string password, AccountStates state, List<Event > schedule,Speciality speciality)
       
        {
            base.Id = 0;
            Name = name;
            Surname = surname;
            base.Email = email;
            base.Password =password;
            this.State = state;
            Schedule = schedule;
            Speciality = speciality;
        }
        public Doctor() 
            :base()
        { 
            
            Name = "";
            Surname = "";
          
            
            State = 0;
            Speciality = 0;
            Schedule = new List<Event>();
        }
    }

    public enum Accessibility
    {
        Accessible = 1,
        PreInaccessible,
        Inaccessible
    }

    public enum Speciality
    {
    Ophthalmologist =1,
    Therapist,
    Surgeon,
    Endoscopist,
    Dentist,
    Neurologist,
    Cardiologist,
    Endocrinologist,
    InfectiousDiseaseSpecialist,
    Gastroenterologist,
    Dermatologist,
    Urologist,
    Gynecologist,
    Otolaryngologist,
    Oncologist,
    Proctologist,
    FunctionalDiagnostics,
    OrthopedicTraumatologist,
    Vertebrologist,
    Psychologist,
    Logopedist,
    Rheumatologist,
    Pulmonologist,
    Physiotherapist,
    Acupuncturist

    }
}
