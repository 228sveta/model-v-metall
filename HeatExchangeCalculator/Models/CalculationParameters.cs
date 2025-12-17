namespace HeatExchangeCalculator.Models
{
    public class CalculationParameters
    {
        // Высота слоя, м
        public double LayerHeight { get; set; } = 3.0;
        
        // Объемный коэффициент теплоотдачи, Вт/(м3 * К)
        public double VolumetricHeatTransferCoefficient { get; set; } = 2440;
        
        // Скорость газа на свободное сечение шахты, м/с
        public double GasVelocity { get; set; } = 0.74;
        
        // Средняя теплоемкость газа, кДж/(м3 * К)
        public double GasHeatCapacity { get; set; } = 1.35;
        
        // Расход материалов кг/с
        public double MaterialFlowRate { get; set; } = 1.68;
        
        // Теплоемкость материалов, кДж/(кг * К)
        public double MaterialHeatCapacity { get; set; } = 1.49;
        
        // Начальная температура материала, °C
        public double InitialMaterialTemperature { get; set; } = 0;
        
        // Начальная температура газа, °C
        public double InitialGasTemperature { get; set; } = 750;
        
        // Диаметр аппарата, м
        public double ApparatusDiameter { get; set; } = 2.2;
    }
}