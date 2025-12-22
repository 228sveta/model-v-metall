using System.Data.SQLite;
using HeatExchangeCalculator.Models;
using System.Text.Json;

namespace HeatExchangeCalculator.Services
{
    public class SimpleDbService
    {
        private readonly string _connectionString = "Data Source=heat_exchange.db";
        
        public SimpleDbService()
        {
            InitializeDatabase();
        }
        
        private void InitializeDatabase()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Calculations (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    SaveDate TEXT NOT NULL,
                    ParametersJson TEXT NOT NULL,
                    ResultJson TEXT NOT NULL
                )";
            command.ExecuteNonQuery();
        }
        
        public void SaveCalculation(string name, CalculationResult result)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Calculations (Name, SaveDate, ParametersJson, ResultJson)
                VALUES ($name, $date, $params, $result)";
            
            command.Parameters.AddWithValue("$name", name);
            command.Parameters.AddWithValue("$date", DateTime.Now.ToString("o"));
            command.Parameters.AddWithValue("$params", 
                JsonSerializer.Serialize(result.Parameters));
            command.Parameters.AddWithValue("$result", 
                JsonSerializer.Serialize(result));
            
            command.ExecuteNonQuery();
        }
        
        public List<SavedCalculation> LoadAllCalculations()
        {
            var calculations = new List<SavedCalculation>();
            
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Calculations ORDER BY SaveDate DESC";
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                calculations.Add(new SavedCalculation
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    SaveDate = DateTime.Parse(reader.GetString(2)),
                    Parameters = JsonSerializer.Deserialize<CalculationParameters>(
                        reader.GetString(3)),
                    Result = JsonSerializer.Deserialize<CalculationResult>(
                        reader.GetString(4))
                });
            }
            
            return calculations;
        }
        
        public SavedCalculation LoadCalculation(int id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Calculations WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new SavedCalculation
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    SaveDate = DateTime.Parse(reader.GetString(2)),
                    Parameters = JsonSerializer.Deserialize<CalculationParameters>(
                        reader.GetString(3)),
                    Result = JsonSerializer.Deserialize<CalculationResult>(
                        reader.GetString(4))
                };
            }
            
            return null;
        }
        
        public void DeleteCalculation(int id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Calculations WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
    }
}