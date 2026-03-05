using FitnessBL.Domein;
using FitnessBL.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FitnessDL
{
    public class RepositorySQL : IRepository
    {
        private string _connectionString;

        public RepositorySQL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ImporteerGegevens(List<Loopsessie> sessies)
        {
            // Maak de queries aan. Waarden die met @ beginnen zijn parameters
            string sessieQuery = "INSERT INTO Loopsessie (sessienummer, datum, klantennummer, duurinminuten, gemiddeldesnelheid) " +
                "OUTPUT Inserted.id VALUES(@sessienummer, @datum, @klantennummer, @duurinminuten, @gemiddeldesnelheid)";

            string intervalQuery = "INSERT INTO Loopinterval (sequentienummer, duurinseconden, loopsnelheid, sessieid) " +
                "VALUES (@sequentienummer, @duurinseconden, @loopsnelheid, @sessieid)";
            foreach (Loopsessie loopsessie in sessies)
            {
                // Start een SqlConnection op
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    // Maak de twee commands aan via je connectie
                    using (SqlCommand sessieCommand = connection.CreateCommand())
                    {
                        using (SqlCommand intervalCommand = connection.CreateCommand())
                        {
                            // Link de query's aan de juiste commands
                            sessieCommand.CommandText = sessieQuery;
                            intervalCommand.CommandText = intervalQuery;

                            // Start een transactie en link ze aan de commands.
                            // We gebruiken een transactie omdat we in twee tabellen tegelijkertijd moeten schrijven
                            SqlTransaction transaction = connection.BeginTransaction();
                            sessieCommand.Transaction = transaction;
                            intervalCommand.Transaction = transaction;

                            try
                            {
                                // Maak de parameters aan.
                                // We hebben telkens maar één loopsessie nodig, dus kunnen we de parameters direct instellen.
                                sessieCommand.Parameters.AddWithValue("@sessienummer", loopsessie.Sessienummer);
                                sessieCommand.Parameters.AddWithValue("@datum", loopsessie.Datum);
                                sessieCommand.Parameters.AddWithValue("@klantennummer", loopsessie.Klantennummer);
                                sessieCommand.Parameters.AddWithValue("@duurinminuten", loopsessie.DuurInMinuten);
                                sessieCommand.Parameters.AddWithValue("@gemiddeldesnelheid", loopsessie.GemiddeldeSnelheid);

                                // We voeren deze command uit en slaan telkens de waarde in de eerste kolom (= het id van de sessie) op
                                int sessieId = (int)sessieCommand.ExecuteScalar();

                                // De parameters moeten we voor elk interval (= meerdere keren in deze loop) aanpassen, dus stellen we ze nog niet in.
                                // Het SessieId zal in deze loop niet meer veranderen tot we naar de volgende Loopsessie gaan, dus die stellen we wel al in.
                                intervalCommand.Parameters.Add("@sequentienummer", SqlDbType.Int);
                                intervalCommand.Parameters.Add("@duurinseconden", SqlDbType.Int);
                                intervalCommand.Parameters.Add("@loopsnelheid", SqlDbType.Float);
                                intervalCommand.Parameters.AddWithValue("@sessieid", sessieId);

                                foreach(Loopinterval interval in loopsessie.Intervallen)
                                {
                                    intervalCommand.Parameters["@sequentienummer"].Value = interval.Sequentienummer;
                                    intervalCommand.Parameters["@duurinseconden"].Value = interval.DuurInSeconden;
                                    intervalCommand.Parameters["@loopsnelheid"].Value = interval.Loopsnelheid;

                                    intervalCommand.ExecuteNonQuery();
                                }

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw ex;
                            }

                        }
                    }
                }
            }
        }
    }
}