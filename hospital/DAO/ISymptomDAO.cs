using hospital.Entities;

namespace hospital.DAO
{
    public interface ISymptomDAO
    {
        public void AddSymptom(List<Symptom> symptoms);
        public List<Symptom> GetAllSymptoms();
    }
}
