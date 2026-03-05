using FitnessBL.Domein;
using FitnessBL.Interfaces;

namespace FitnessBL.Beheerders
{
    public class ImportBeheerder
    {
        private IBestandslezer _lezer;
        private IRepository _repo;

        public ImportBeheerder(IBestandslezer lezer, IRepository repo)
        {
            _lezer = lezer;
            _repo = repo;
        }

        public void ImporteerGegevens()
        {
            List<Loopsessie> sessies = _lezer.LeesBestandIn();
            _repo.ImporteerGegevens(sessies);
        }
    }
}
