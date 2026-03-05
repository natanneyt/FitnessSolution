using FitnessBL.Interfaces;
using FitnessBL.Beheerders;
using FitnessUtil.Factories;
using Microsoft.Extensions.Configuration;

namespace FitnessUI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();

            string connectionString = config.GetConnectionString("SQLServerConnection");
            string sourceFilePath = config.GetSection("AppSettings")["sourceFilePath"];
            string testFilePath = config.GetSection("AppSettings")["testFilePath"];
            string errorLogPath = config.GetSection("AppSettings")["errorLogPath"];
            string textFileType = config.GetSection("AppSettings")["textFileType"];
            string databaseType = config.GetSection("AppSettings")["databaseType"];

            //Console.WriteLine(File.ReadAllText(testFilePath));
            //Console.WriteLine(File.Exists(testFilePath));

            IBestandslezer lezer = BestandslezerFactory.GeefBestandslezer(textFileType, sourceFilePath, errorLogPath);
            IRepository repo = RepositoryFactory.GeefRepository(databaseType, connectionString);
            ImportBeheerder beheerder = new ImportBeheerder(lezer, repo);
            
            // Nadat je hebt geïmporteerd, verwijder je dit zodat je niet alles meerdere keren toevoegt.
            // beheerder.ImporteerGegevens();

        }
    }
}
