using hospital.DAO;
using hospital.Entities;
using hospital.Exceptions;

namespace hospital.Services
{
    public class DrugService
    {
        IDrugDAO _drugDAO;

        public DrugService(IDrugDAO drugDAO)
        {
            _drugDAO = drugDAO;
        }


        public List<Drug> GetAllDrugs()
        {
            try
            {
                return _drugDAO.GetAllDrugs();
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
            catch (NoSuchRecord e)
            {
                throw new NoSuchRecord(e.Message, e);
            }
        }

    }
}
