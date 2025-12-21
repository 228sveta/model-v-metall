using System;
using HeatExchangeCalculator.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HeatExchangeCalculator.Services
{
    public class CalculationStorageService
    {
        private readonly string _storagePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public CalculationStorageService()
        {
            // Папка для хранения данных
            _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "calculations.json");
            
            // Создаем папку если нет
            var directory = Path.GetDirectoryName(_storagePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // Настройки JSON
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        // Сохранить расчет
        public void SaveCalculation(SavedCalculation calculation)
        {
            var calculations = LoadAllCalculations();
            calculation.Id = calculations.Any() ? calculations.Max(c => c.Id) + 1 : 1;
            calculation.SaveDate = DateTime.Now;
            
            calculations.Add(calculation);
            SaveAllCalculations(calculations);
        }

        // Загрузить все расчеты
        public List<SavedCalculation> LoadAllCalculations()
        {
            if (!File.Exists(_storagePath))
            {
                return new List<SavedCalculation>();
            }

            try
            {
                var json = File.ReadAllText(_storagePath);
                return JsonSerializer.Deserialize<List<SavedCalculation>>(json, _jsonOptions) 
                    ?? new List<SavedCalculation>();
            }
            catch
            {
                return new List<SavedCalculation>();
            }
        }

        // Загрузить расчет по ID
        public SavedCalculation? LoadCalculation(int id)
        {
            var calculations = LoadAllCalculations();
            return calculations.FirstOrDefault(c => c.Id == id);
        }

        // Удалить расчет
        public void DeleteCalculation(int id)
        {
            var calculations = LoadAllCalculations();
            var calculation = calculations.FirstOrDefault(c => c.Id == id);
            
            if (calculation != null)
            {
                calculations.Remove(calculation);
                SaveAllCalculations(calculations);
            }
        }

        // Сохранить все расчеты
        private void SaveAllCalculations(List<SavedCalculation> calculations)
        {
            var json = JsonSerializer.Serialize(calculations, _jsonOptions);
            File.WriteAllText(_storagePath, json);
        }
    }

    // ТОЛЬКО ОДНО ОПРЕДЕЛЕНИЕ SavedCalculation!
    [Serializable]
    public class SavedCalculation
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime SaveDate { get; set; }
        public CalculationParameters Parameters { get; set; } = new CalculationParameters();
        public CalculationResult Result { get; set; } = new CalculationResult();
    }
}