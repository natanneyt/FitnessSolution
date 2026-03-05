using FitnessBL.Domein;

namespace FitnessBL.Interfaces
{
    public interface IBestandslezer
    {
        // Deze interface zorgt er voor dat we vanuit de UI (die de Datalaag niet kent)
        // toch een bestandslezer kunnen aanmaken, aangezien deze interface in de Businesslaag zit.
        List<Loopsessie> LeesBestandIn();
    }
}
