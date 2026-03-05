using FitnessBL.Domein;
using FitnessBL.Interfaces;

namespace FitnessBL.Beheerders
{
    public class FitnessBeheerder
    {
        private IRepository _repo;

        public FitnessBeheerder(IRepository repo)
        {
            _repo = repo;
        }

        public List<Loopsessie> GeefSessiesVanDag(DateTime dateTime)
        {
            return _repo.GeefSessiesVanDag(dateTime);
        }

        public List<Loopsessie> GeefSessiesVanKlant(int klantennummer)
        {
            return _repo.GeefSessiesVanKlant(klantennummer);
        }
    }
}
