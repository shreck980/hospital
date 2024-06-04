using System.ComponentModel;
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

        public Doctor(long id, string name, string surname, string email, string password, AccountStates state, List<Event> schedule, Speciality speciality)
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
        [Description("Офтальмолог")]
        Ophthalmologist = 1,
        [Description("Терапевт")]
        Therapist,
        [Description("Хірург")]
        Surgeon,
        [Description("Ендоскопіст")]
        Endoscopist,
        [Description("Стоматолог")]
        Dentist,
        [Description("Невролог")]
        Neurologist,
        [Description("Кардіолог")]
        Cardiologist,
        [Description("Ендокринолог")]
        Endocrinologist,
        [Description("Інфекціоніст")]
        InfectiousDiseaseSpecialist,
        [Description("Гастроентеролог")]
        Gastroenterologist,
        [Description("Дерматолог")]
        Dermatologist,
        [Description("Уролог")]
        Urologist,
        [Description("Гінеколог")]
        Gynecologist,
        [Description("Отоларинголог")]
        Otolaryngologist,
        [Description("Онколог")]
        Oncologist,
        [Description("Проктолог")]
        Proctologist,
        [Description("Лікар функціональної діагностики")]
        FunctionalDiagnostics,
        [Description("Ортопед-травмолог")]
        OrthopedicTraumatologist,
        [Description("Вертебролог")]
        Vertebrologist,
        [Description("Психолог")]
        Psychologist,
        [Description("Логопед")]
        Logopedist,
        [Description("Ревматолог")]
        Rheumatologist,
        [Description("Пульмонолог")]
        Pulmonologist,
        [Description("Фізіотерапевт")]
        Physiotherapist,
        [Description("Голкорефлексотерапевт")]
        Acupuncturist

    }
}
