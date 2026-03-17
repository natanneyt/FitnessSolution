using FitnessBL.Interfaces;
using FitnessBL.Beheerders;
using FitnessUtil.Factories;
using Microsoft.Extensions.Configuration;
using FitnessBL.Domein;

namespace FitnessUI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
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
            ImportBeheerder importBeheerder = new ImportBeheerder(lezer, repo);
            FitnessBeheerder fitnessBeheerder = new FitnessBeheerder(repo);

            List<Loopsessie> sessiesVanKlant = fitnessBeheerder.GeefSessiesVanKlant(6);

            foreach (Loopsessie sessie in sessiesVanKlant)
            {
                Console.WriteLine($"{sessie}\n");
            }

            List<Loopsessie> sessiesVanDag = fitnessBeheerder.GeefSessiesVanDag(new DateTime(2021, 11, 4));
            foreach (Loopsessie sessie in sessiesVanDag)
            {
                Console.WriteLine($"{sessie}\n");
            }

            // Nadat je hebt geïmporteerd, verwijder je dit zodat je niet alles meerdere keren toevoegt.
            // importBeheerder.ImporteerGegevens();

        }
    }
}
