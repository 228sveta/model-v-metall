namespace HeatExchangeCalculator.Models
{
    public class CalculationResult
    {
        // Основные расчетные величины
        public double CrossSectionalArea { get; set; }          // Площадь сечения аппарата
        public double HeatCapacityRatio { get; set; }           // Отношение теплоемкостей (m)
        public double TotalRelativeHeight { get; set; }         // Полная относительная высота (Y0)
        public double FullRelativeHeight { get; set; }          // Полная относительная высота слоя
        
        // Данные по точкам высоты (0, 0.5, 1, 1.5, 2, 2.5, 3 м)
        public List<PointData> Points { get; set; } = new List<PointData>();
        
        // Исходные параметры
        public CalculationParameters Parameters { get; set; } = new CalculationParameters();
    }

    public class PointData
    {
        public double Height { get; set; }                      // Высота y, м
        public double Y { get; set; }                           // Y = (αV * y)/(wг * Cг * 1000)
        public double Theta1 { get; set; }                      // 1 - exp[(m-1)Y/m]
        public double Theta2 { get; set; }                      // 1 - m*exp[(m-1)Y/m]
        public double DimensionlessMaterialTemp { get; set; }   // ϑ = θ1 / полная_отн_высота
        public double DimensionlessGasTemp { get; set; }        // θ = θ2 / полная_отн_высота
        public double MaterialTemperature { get; set; }         // t = t' + (T' - t') * ϑ
        public double GasTemperature { get; set; }              // T = t' + (T' - t') * θ
        public double TemperatureDifference { get; set; }       // ΔT = t - T
    }
}