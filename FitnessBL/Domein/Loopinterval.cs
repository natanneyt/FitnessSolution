using FitnessBL.Exceptions;

namespace FitnessBL.Domein
{
    public class Loopinterval
    {
        private int _sequentienummer;
        public int Sequentienummer
        {
            get
            {
                return _sequentienummer;
            }
            set
            {
                if (value <= 0) throw new FitnessException($"Sequentienummer te klein (min: 1): {value}");
                else _sequentienummer = value;
            }
        }

        private int _duurInSeconden;
        public int DuurInSeconden
        {
            get
            {
                return _duurInSeconden;
            }
            set
            {
                if (value < 5) throw new FitnessException($"DuurInSeconden te klein (min: 5): {value}");
                else if (value > 10800) throw new FitnessException($"DuurInSeconden te groot (max: 10800): {value}");
                else _duurInSeconden = value;
            }
        }

        private double _loopsnelheid;
        public double Loopsnelheid
        {
            get
            {
                return _loopsnelheid;
            }
            set
            {
                if (value < 5) throw new FitnessException($"Loopsnelheid te klein (min: 5): {value}");
                else if (value > 22) throw new FitnessException($"Loopsnelheid te groot (max: 22): {value}");
                else _loopsnelheid = value;
            }
        }

        public Loopinterval(int sequentienummer, int duurInSeconden, double loopsnelheid)
        {
            // Fouten in setters worden eerst naar de constructor teruggestuurd.
            // We vangen deze hier telkens op en steken ze uiteindelijk in 1 FitnessException-object.
            List<string> fouten = new List<string>();

            try
            {
                Sequentienummer = sequentienummer;
            }
            catch (FitnessException exception)
            {
                fouten.Add(exception.Message);
            }

            try
            {
                DuurInSeconden = duurInSeconden;
            }
            catch (FitnessException exception)
            {
                fouten.Add(exception.Message);
            }

            try
            {
                Loopsnelheid = loopsnelheid;
            }
            catch (FitnessException exception)
            {
                fouten.Add(exception.Message);
            }

            if (fouten.Count > 0)
            {
                throw new FitnessException(fouten);
            }
        }

        public override string ToString()
        {
            return $"Sequentienummer: {Sequentienummer} - Duur in seconden: {DuurInSeconden} - Loopsnelheid: {Loopsnelheid}";
        }

        // We gebruiken een HashSet die geen twee dezelfde sequentienummers toelaat.
        public override bool Equals(object? obj)
        {
            if (obj is Loopinterval anderInterval)
            {
                return Sequentienummer.Equals(anderInterval.Sequentienummer);
            }
            else throw new Exception("Objecten zijn niet vergelijkbaar.");
        }

        public override int GetHashCode()
        {
            return Sequentienummer.GetHashCode();
        }
    }
}
