namespace hospital.Entities
{
    public class Doctor
    {
        public uint Id {  get; set; }
      
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

        public Accessibility Accessibility { get; set; }


        public Doctor(uint id, string name, string surname, string email, string password, Accessibility accessibility)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
            Password = PasswordManager.HashPassword(password);
            Accessibility = accessibility;
        }
    }

    public enum Accessibility
    {
        Accessible = 1,
        PreInaccessible,
        Inaccessible
    }
}
