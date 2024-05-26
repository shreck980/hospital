using hospital.Entities;

namespace hospital.DAO
{
    public interface IPatientDAO
    {
        public void GetAllPatients();
        public Patient GetPatientByEmail(string email);
        public Patient GetPatientById(uint id);
        public Patient GetPatientByIdSecure(uint id);
        public void AddPatientWithoutСonfirmation(Patient patient);
        public void UpdatePatientWithСonfirmation(Patient patient);
        public void UpdatePatientFamilyDoctor(Patient patient);
        public Patient GetPatientMinDataById(uint id);
        public void SetAccountState(AccountStates state);
    }
}
