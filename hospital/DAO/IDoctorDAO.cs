using hospital.Entities;

namespace hospital.DAO
{
    public interface IDoctorDAO
    {
        public void AddDoctor(Doctor d);
        public Doctor GetDoctorById(uint id);
        public List<Doctor> GetDoctorsBySpeciality(Speciality speciality);
        public Doctor GetDoctorByEmail(string email);
        public List<Doctor> GetAllDoctorsExceptTherapist();
        public List<Doctor> GetAllDoctorsForPatient();
        public string GetSpeciality(Speciality speciality);
        public Doctor GetDoctorByIdMin(uint id);
    }
}
