using FitnessBL.Exceptions;

namespace FitnessBL.Domein
{
    public class Loopsessie
    {
        private int _sessienummer;
        public int Sessienummer
        {
            get
            {
                return _sessienummer;
            }
            set
            {
                if (value <= 0) throw new FitnessException($"Sessienummer te klein (min: 1): {value}");
                else _sessienummer = value;
            }
        }

        public DateTime Datum { get; set; }

        private int _klantennummer;
        public int Klantennummer
        {
            get
            {
                return _klantennummer;
            }
            set
            {
                if (value <= 0) throw new FitnessException($"Klantennummer te klein (min: 1): {value}");
                else _klantennummer = value;
            }
        }

        private int _duurInMinuten;
        public int DuurInMinuten
        {
            get
            {
                return _duurInMinuten;
            }
            set
            {
                if (value < 5) throw new FitnessException($"DuurInMinuten te klein (min: 5): {value}");
                else if (value > 180) throw new FitnessException($"DuurInMinuten te groot (max: 180): {value}");
                else _duurInMinuten = value;
            }
        }

        private double _gemiddeldeSnelheid;
        public double GemiddeldeSnelheid
        {
            get
            {
                return _gemiddeldeSnelheid;
            }
            set
            {
                if (value < 5) throw new FitnessException($"GemiddeldeSnelheid te klein (min: 5): {value}");
                else if (value > 22) throw new FitnessException($"GemiddeldeSnelheid te groot (max: 22): {value}");
                else _gemiddeldeSnelheid = value;
            }
        }

        // We gebruiken een HashSet die geen twee dezelfde sequentienummers toelaat.
        // Vergeet de Equals()- en GetHashCode()-methodes van Loopinterval niet te overriden.
        // De setter is private aangezien we deze lijst binnen deze klasse met een methode aanvullen.
        public HashSet<Loopinterval> Intervallen { get; private set; } = new HashSet<Loopinterval>();

        public Loopsessie(int sessienummer, DateTime datum, int klantennummer, int duurInMinuten, double gemiddeldeSnelheid)
        {            
            // Fouten in setters worden eerst naar de constructor teruggestuurd.
            // We vangen deze hier telkens op en steken ze uiteindelijk in 1 FitnessException-object.
            List<string> fouten = new List<string>();
            try
            {
                Sessienummer = sessienummer;
            }
            catch (FitnessException exception)
            {
                fouten.Add(exception.Message);
            }

            try
            {
                Datum = datum;
            }
            catch (FitnessException exception)
            {
                fouten.Add(exception.Message);
            }

            try
            {
                Klantennummer = klantennummer;
            }
            catch (FitnessException exception)
            {
                fouten.Add(exception.Message);
            }

            try
            {
                DuurInMinuten = duurInMinuten;
            }
            catch (FitnessException exception)
            {
                fouten.Add(exception.Message);
            }

            try
            {
                GemiddeldeSnelheid = gemiddeldeSnelheid;
            }
            catch (FitnessException exception)
            {
                fouten.Add(exception.Message);
            }

            if (fouten.Count > 0) throw new FitnessException(fouten);
        }

        // We gaan telkens eerst een Loopsessie aanmaken en daarna elk Loopinterval apart toevoegen.
        public void VoegIntervalToe(Loopinterval interval)
        {
            Intervallen.Add(interval);
        }

        public override string ToString()
        {
            return $"Sessie {Sessienummer} - {Datum} - Klant: {Klantennummer} - Duur in minuten: {DuurInMinuten} - Gemiddelde snelheid: {GemiddeldeSnelheid}";
        }
    }
}
