using System.ComponentModel.DataAnnotations;

namespace hospital.Entities
{
    public class Account
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Електронна адреса")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
      
        private string _password;
        [Display(Name = "Пароль")]
        public string Password
        {
            get { return _password; }
            set
            {

                _password = value;
            }
        }
        public bool IsLogged { get; set; } = false;
        public Account(long id, string email, string password)
        {
            Id = id;
            Email = email;
            Password = password;
        }
        public Account()
        {
            Id = 0;
            Email = "";
            Password = "";
        }
    }
}
