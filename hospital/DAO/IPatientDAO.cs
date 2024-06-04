using hospital.Entities;

namespace hospital.DAO
{
    public interface IPatientDAO
    {
        public void GetAllPatients();
        public Patient GetPatientByEmail(string email);
        public Patient GetPatientByEmailUnverified(string email);
        public Patient GetPatientById(long id);
        public Patient GetPatientByIdSecure(long id);
        public void AddPatientWithoutСonfirmation(Patient patient);
        public void UpdatePatientWithСonfirmation(Patient patient);
        public void UpdatePatientFamilyDoctor(Patient patient);
        public Patient GetPatientMinDataById(long id);
        public void SetAccountState(AccountStates state,long id);
    }
}
