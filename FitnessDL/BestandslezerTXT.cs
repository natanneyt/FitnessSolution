using FitnessBL.Domein;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;

namespace FitnessDL
{
    public class BestandslezerTXT : IBestandslezer
    {
        private string _dataPad;
        private string _foutenPad;

        public BestandslezerTXT(string dataPad, string foutenPad)
        {
            _dataPad = dataPad;
            _foutenPad = foutenPad;
        }

        public List<Loopsessie> LeesBestandIn()
        {
            // We slaan de objecten tijdelijk op in een dictionary
            // zodat we snel op sessienummer kunnen zoeken wanneer we een interval moeten toevoegen
            Dictionary<int, Loopsessie> loopsessies = new Dictionary<int, Loopsessie>();

            using (StreamWriter streamWriter = new StreamWriter(_foutenPad))
            {
                using (StreamReader streamReader = new StreamReader(_dataPad))
                {
                    int huidigeLijn = 1;
                    while(!streamReader.EndOfStream)
                    {
                        try
                        {
                            // We lezen eerst de volledige lijn in
                            // en slaan dan de waarden binnen de haakjes van de insert op.
                            string insertLijn = streamReader.ReadLine();
                            char[] haakjes = { '(', ')' };
                            string lijn = insertLijn.Split(haakjes)[1];
                            string[] lijnSecties = lijn.Split(',');

                            int sessienummer = int.Parse(lijnSecties[0]);
                            if (!loopsessies.ContainsKey(sessienummer))
                            {
                                loopsessies.Add(sessienummer, new Loopsessie(sessienummer, DateTime.Parse(lijnSecties[1].Replace("'", newValue: string.Empty)), int.Parse(lijnSecties[2]), int.Parse(lijnSecties[3]), double.Parse(lijnSecties[4].Replace('.',','))));
                            }

                            loopsessies[int.Parse(lijnSecties[0])].VoegIntervalToe(new Loopinterval(int.Parse(lijnSecties[5]), int.Parse(lijnSecties[6]), double.Parse(lijnSecties[7].Replace('.',','))));
                        }
                        catch(FitnessException exception)
                        {
                            streamWriter.WriteLine($"Lijn {huidigeLijn}: { string.Join(" | ", exception.Fouten)}");
                        }
                        huidigeLijn++;
                    }
                }
            }
            return loopsessies.Values.ToList();
        }
    }
}
