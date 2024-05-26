using hospital.Entities;

namespace hospital.DAO
{
    public interface IMedicalCardDAO
    {
        public void AddMedicalCard(uint patientId);

        public void AddAppointmentRecord(MedicalCard card);
        public MedicalCard GetMedicalCardEmpty(uint patientId);
        public MedicalCard GetMedicalCardPatient(uint patientId);
        public (EMR, EHR) GetAppointmentDetailsPatient(uint appointment);
    }
}
