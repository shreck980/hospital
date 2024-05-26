using hospital.Entities;

namespace hospital.DAO
{
    public interface IPaymentDAO
    {
        public Payment? GetPaymentById(uint id);
    }
}
