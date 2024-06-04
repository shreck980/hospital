using hospital.Entities;

namespace hospital.DAO
{
    public interface IMedicalCardDAO
    {
        public void AddMedicalCard(long patientId);

        public void AddAppointmentRecord(MedicalCard card);
        public MedicalCard GetMedicalCardEmpty(long patientId);
        public MedicalCard GetMedicalCardPatient(long patientId);
        public (EMR, EHR) GetAppointmentDetailsPatient(long appointment);
        public void DeleteReferral(long id);
        public void UpdateReferralState(long referralId, int state,long?appointment);
        public long GetReferralIdByAppointmentId(long id);
    }
}
