namespace FitnessBL.Exceptions
{
    public class FitnessException : Exception
    {
        public List<string> Fouten { get; set; }

        // Hier combineren we alle foutberichten in 1 FitnessException-object
        public FitnessException(List<string> fouten)
        {
            Fouten = fouten;
        }

        // Hier vangen we eerst elke fout individueel op.
        public FitnessException(string? foutmelding) : base(foutmelding) { }
    }
}