using System.Numerics;

namespace hospital.Entities
{
    public class Payment
    {
        public uint Id {  get; set; }
        public decimal Price { get; set; }
        public DateTime DateIssued { get; set; } = DateTime.Now;
        public DateTime? DatePaid { get; set; }
        public Patient Patient { get; set; }

        public Payment()
        {
            Patient = new Patient();
            Price = 0;
            Id = 0;
        }
        public Payment(uint id, decimal price, DateTime dateIssued, DateTime datePaid, Patient patient)
        {
            Id = id;
            Price = price;
            DateIssued = dateIssued;
            DatePaid = datePaid;
            Patient = patient;
        }

        public Payment(uint id, decimal price,  DateTime dateIssued, Patient patient)
        {
            Id = id;
            Price = price;
            DateIssued = dateIssued;
            Patient = patient;
        }
    }
}
