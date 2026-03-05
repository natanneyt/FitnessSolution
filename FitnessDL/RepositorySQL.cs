using FitnessBL.Interfaces;

namespace FitnessDL
{
    public class RepositorySQL : IRepository
    {
        private string _connectionString;

        public RepositorySQL(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}