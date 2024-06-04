using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace hospital.Entities
{
   
 
    public class Patient : Account
    {

        [Display(Name = "Ім'я")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Surname is required")]
        [Display(Name = "Прізвище")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Birthday is required")]
        [DataType(DataType.Date)]
        [Display(Name = "День народження")]
      
        public DateTime Birthday { get; set; }
        [ValidateNever]
        public AccountStates State { get; set; }
        [ValidateNever]
        public Doctor FamilyDoctor { get; set; }
        [ValidateNever]
        public MedicalCard MedicalCard { get; set; }
        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Адреса")]
        public string Address { get; set; }
      

        public Patient(long id, string name, string surname, string email, string password, DateTime birthday, AccountStates state, Doctor familyDoctor, MedicalCard medicalCard, string address)
            :base(id,email,password)
        {
            
            Name = name;
            Surname = surname;
            
           
            Birthday = birthday;
            State = state;
            FamilyDoctor = familyDoctor;
            MedicalCard = medicalCard;
            Address = address;
        }
        public Patient(string name, string surname, string email, string password, DateTime birthday, string address)
            :base()
        {     
            Name = name;
            Surname = surname;
            Birthday = birthday;
            State = AccountStates.Unverified;
            FamilyDoctor = new Doctor();
            MedicalCard = new MedicalCard();
            Address = address;
        }
        public Patient()
            :base()
        {
            Name = "";
            Surname = "";
            Birthday = DateTime.Today;
            State = AccountStates.Unverified;
            FamilyDoctor = new Doctor();
            MedicalCard = new MedicalCard();
            Address = "";
         
        }

     
        



        public override string ToString()
        {
            return $"Name: {Name}, \nSurname: {Surname}, \nEmail: {Email}, \nPassword: {Password}, \nBirthday: {Birthday}, \nState: {State}, \nFamilyDoctor: {FamilyDoctor}, \nMedicalCard: {MedicalCard}";
        }

        
    }

    public enum AccountStates
    {
        Unverified = 1,
        Verified,
        Active,
        PreActive,
        Frozen,
        PreRemoved
        
    }
}
