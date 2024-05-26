using hospital.Entities;

namespace hospital.DAO
{
    public interface IDrugDAO
    {

        public void AddDrug(List<Drug> drugs);
        public List<Drug> GetAllDrugs();
    }
}
