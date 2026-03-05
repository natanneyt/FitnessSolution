using FitnessBL.Interfaces;
using FitnessDL;

namespace FitnessUtil.Factories
{
    public static class BestandslezerFactory
    {
        public static IBestandslezer GeefBestandslezer(string bestandstype, string pad, string foutenPad)
        {
            switch(bestandstype.Trim().ToUpper())
            {
                case "TXT":
                    {
                        return new BestandslezerTXT(pad, foutenPad);
                    }
                default:
                    {
                        throw new Exception("Ongeldig bestandstype.");
                    }
            }
        }
    }
}
