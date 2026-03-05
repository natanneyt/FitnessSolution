using FitnessBL.Domein;

namespace FitnessBL.Interfaces
{
    public interface IRepository
    {
        // Deze interface zorgt er voor dat we vanuit de UI (die de Datalaag niet kent)
        // toch een repository kunnen aanmaken, aangezien deze interface in de Businesslaag zit.
        void ImporteerGegevens(List<Loopsessie> sessies);

        List<Loopsessie> GeefSessiesVanKlant(int klantennummer);
        List<Loopsessie> GeefSessiesVanDag(DateTime dateTime);
    }
}
