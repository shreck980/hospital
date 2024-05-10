namespace hospital.Entities
{
    public class Referral
    {

        public Doctor Doctor { get; set; }
        public DateTime ExpirationDate { get; set; }

        public Referral(Doctor doctor, DateTime expirationDate)
        {
            Doctor = doctor;
            ExpirationDate = expirationDate;
        }
    }
}
