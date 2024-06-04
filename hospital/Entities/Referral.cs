using System.ComponentModel;

namespace hospital.Entities
{
    public class Referral
    {
        public long Id { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime ExpirationDate { get; set; }
        public ReferralState State { get; set; }
        public long AppointmetnId {  get; set; }

        public Referral(Doctor doctor, DateTime expirationDate)
        {
            Id = 0;
            Doctor = doctor;
            ExpirationDate = expirationDate;
        }
        public Referral(long id,Doctor doctor, DateTime expirationDate)
        {
            Id = id;
            Doctor = doctor;
            ExpirationDate = expirationDate;
            State = ReferralState.Issued;
        }


        public Referral()
        {
            Id = 0;
            Doctor = new Doctor();
            ExpirationDate = DateTime.MinValue;
            State = ReferralState.Issued;
            AppointmetnId = 0;
        }
    }

    public enum ReferralState {

        [Description("Направлення виписано лікарем")]
        Issued=1,
        [Description("На прийом записаний по направленню")]
        Scheduled,
        [Description("Прийом по направленню відвідано")]
        Visited
    }

}
