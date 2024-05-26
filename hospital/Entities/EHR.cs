namespace hospital.Entities
{
    public class EHR
    {
        public uint Id { get; set; }
        public string ResultOfExamination { get; set; }
        public List<Symptom> Symptoms { get; set; }
        public List<Drug> Drugs { get; set; }

        public EHR(string results, List<Drug> drugs, List<Symptom> symptoms)
        {
            Symptoms = symptoms;
            Drugs = drugs;
            ResultOfExamination = results;

        }
        public EHR()
        {
            Symptoms = new List<Symptom>();
            Drugs = new List<Drug>();
            ResultOfExamination = "";
        }
    }
}
