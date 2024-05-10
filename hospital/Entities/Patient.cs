using System.Net;
using System.Xml.Linq;

namespace hospital.Entities
{
    public class Patient
    {
        private static uint lastId;
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = PasswordManager.HashPassword(value);
            }
        }

        public DateTime Birthday { get; set; }
        public AccountStates State { get; set; }
        public Doctor FamilyDoctor { get; set; }
        public MedicalCard MedicalCard { get; set; }

        public string Address { get; set; }

        public Patient(uint id, string name, string surname, string email, string password, DateTime birthday, AccountStates state, Doctor familyDoctor, MedicalCard medicalCard, string address)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
            _password = PasswordManager.HashPassword(password);
            Birthday = birthday;
            State = state;
            FamilyDoctor = familyDoctor;
            MedicalCard = medicalCard;
            Address = address;
        }
        public Patient(string name, string surname, string email, string password, DateTime birthday, string address)
        {
            Id = GenerateId();
            Name = name;
            Surname = surname;
            Email = email;
            _password = PasswordManager.HashPassword(password);
            Birthday = birthday;
            State = AccountStates.Unverified;
            FamilyDoctor = new Doctor();
            MedicalCard = new MedicalCard();
            Address = address;
        }
        public Patient()
        {
            Name = "";
            Surname = "";
            Email = "";
            _password = "";
            Birthday = DateTime.Today;
            State = AccountStates.Unverified;
            FamilyDoctor = new Doctor();
            MedicalCard = new MedicalCard();
            Address = "";
            Id = 0;
        }


        public override string ToString()
        {
            return $"Name: {Name}, \nSurname: {Surname}, \nEmail: {Email}, \nPassword: {Password}, \nBirthday: {Birthday}, \nState: {State}, \nFamilyDoctor: {FamilyDoctor}, \nMedicalCard: {MedicalCard}";
        }

        private static uint GenerateId()
        {
            return ++lastId;
        }

    }

    public enum AccountStates
    {
        Unverified = 1,
        Verified,
        Active,
        Blocked,
        PreRemoved
    }
}
