namespace hospital.Entities
{
    public class EMR
    {

        public uint Id { get; set; }
        public Referral Referral {  get; set; }
        public EMR(Referral referral) { 
            this.Referral = referral;
        }

        public EMR()
        {
            this.Referral = new Referral();
        }
    }
}
