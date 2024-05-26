namespace hospital.Entities
{
    public class Referral
    {
        public uint Id { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime ExpirationDate { get; set; }

        public Referral(Doctor doctor, DateTime expirationDate)
        {
            Id = 0;
            Doctor = doctor;
            ExpirationDate = expirationDate;
        }
        public Referral(uint id,Doctor doctor, DateTime expirationDate)
        {
            Id = id;
            Doctor = doctor;
            ExpirationDate = expirationDate;
        }


        public Referral()
        {
            Id = 0;
            Doctor = new Doctor();
            ExpirationDate = DateTime.MinValue;
        }
    }
}
