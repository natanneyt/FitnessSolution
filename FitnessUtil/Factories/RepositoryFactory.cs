using FitnessBL.Interfaces;
using FitnessDL;

namespace FitnessUtil.Factories
{
    public static class RepositoryFactory
    {
        public static IRepository GeefRepository(string databaseType, string connectionString)
        {
            switch(databaseType.Trim().ToUpper())
            {
                case "SQL":
                    {
                        return new RepositorySQL(connectionString);
                    }
                default:
                    {
                        throw new Exception("Ongeldig databasetype.");
                    }
            }
        }
    }
}
