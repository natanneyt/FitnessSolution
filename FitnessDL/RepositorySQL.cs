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
            // SET IDENTITY_INSERT Loopsessie ON; => dit is nodig aangezien ik sessienummer als id heb genomen.
            // Als je een nieuwe id-kolom aanmaakt, is dit niet nodig. Ik vond een extra kolom hier overbodig.
            string sessieQuery = "SET IDENTITY_INSERT Loopsessie ON; INSERT INTO Loopsessie (id, datum, klantennummer, duurinminuten, gemiddeldesnelheid) " +
                "OUTPUT Inserted.id VALUES(@sessienummer, @datum, @klantennummer, @duurinminuten, @gemiddeldesnelheid)";

            string intervalQuery = "INSERT INTO Loopinterval (sequentienummer, duurinseconden, loopsnelheid, sessieid) " +
                "VALUES (@sequentienummer, @duurinseconden, @loopsnelheid, @sessieid)";

            // Start een SqlConnection op
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (Loopsessie loopsessie in sessies)
                {
                    // Maak de twee commands aan via je connectie
                    using (SqlCommand sessieCommand = connection.CreateCommand())
                    {
                        using (SqlCommand intervalCommand = connection.CreateCommand())
                        {
                            // Link de queries aan de juiste commands
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

                                foreach(Loopinterval interval in loopsessie.Intervals)
                                {
                                    // Voor elk interval stellen we de parameters in en schrijven we individueel naar de databank.
                                    intervalCommand.Parameters["@sequentienummer"].Value = interval.Sequentienummer;
                                    intervalCommand.Parameters["@duurinseconden"].Value = interval.DuurInSeconden;
                                    intervalCommand.Parameters["@loopsnelheid"].Value = interval.Loopsnelheid;

                                    // We hebben geen output nodig, dus kunnen we de command gewoon uitvoeren.
                                    intervalCommand.ExecuteNonQuery();
                                }

                                // Als alles goed is verlopen kan je nu je transaction doorsturen
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                // Als er iets fout liep, moet je de transaction nog terugdraaien.
                                transaction.Rollback();
                                throw ex;
                            }

                        }
                    }
                }
            }
        }

        public List<Loopsessie> GeefSessiesVanKlant(int klantennummer)
        {
            Dictionary<int, Loopsessie> loopsessies = new Dictionary<int, Loopsessie>();

            string query = "SELECT s.id, s.datum, s.klantennummer, s.duurinminuten, s.gemiddeldesnelheid, " +
                "i.sequentienummer, i.duurinseconden, i.loopsnelheid " +
                "FROM Loopsessie s " +
                "JOIN Loopinterval i ON s.id = i.sessieid " +
                "WHERE s.klantennummer = @klantennummer";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@klantennummer", klantennummer);

                    SqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        int sessienummer = (int)reader["id"];
                        if (!loopsessies.ContainsKey(sessienummer))
                        {
                            loopsessies.Add(sessienummer, new Loopsessie(sessienummer, (DateTime)reader["datum"], (int)reader["klantennummer"], (int)reader["duurinminuten"], (double)reader["gemiddeldesnelheid"]));
                        }
                        loopsessies[sessienummer].Intervals.Add(new Loopinterval((int)reader["sequentienummer"], (int)reader["duurinseconden"], (double)reader["loopsnelheid"]));
                    }
                }
            }

            return loopsessies.Values.ToList();
        }

        public List<Loopsessie> GeefSessiesVanDag(DateTime dateTime)
        {
            Dictionary<int, Loopsessie> loopsessies = new Dictionary<int, Loopsessie>();

            string query = "SELECT s.id, s.datum, s.klantennummer, s.duurinminuten, s.gemiddeldesnelheid, " +
                "i.sequentienummer, i.duurinseconden, i.loopsnelheid " +
                "FROM Loopsessie s JOIN Loopinterval i ON s.id = i.sessieid " +
                "WHERE DATEDIFF(day, s.datum, @datum) = 0";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@datum", $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day}");

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int sessienummer = (int)reader["id"];
                        if (!loopsessies.ContainsKey(sessienummer))
                        {
                            loopsessies.Add(sessienummer, new Loopsessie(sessienummer, (DateTime)reader["datum"], (int)reader["klantennummer"], (int)reader["duurinminuten"], (double)reader["gemiddeldesnelheid"]));
                        }
                        loopsessies[sessienummer].Intervals.Add(new Loopinterval((int)reader["sequentienummer"], (int)reader["duurinseconden"], (double)reader["loopsnelheid"]));
                    }
                }
            }

            return loopsessies.Values.ToList();
        }
    }
}