using System.ComponentModel.DataAnnotations;

namespace HeatExchangeCalculator.Models
{
    public class CalculationParameters
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(0.1, 10.0, ErrorMessage = "Высота слоя должна быть от 0.1 до 10 м")]
        [Display(Name = "Высота слоя, м")]
        public double LayerHeight { get; set; } = 3.0;
        
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(100, 10000, ErrorMessage = "Коэффициент должен быть от 100 до 10000 Вт/(м³·К)")]
        [Display(Name = "Объемный коэффициент теплоотдачи, Вт/(м³·К)")]
        public double VolumetricHeatTransferCoefficient { get; set; } = 2440;
        
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(0.1, 5.0, ErrorMessage = "Скорость газа должна быть от 0.1 до 5 м/с")]
        [Display(Name = "Скорость газа, м/с")]
        public double GasVelocity { get; set; } = 0.74;
        
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(0.5, 3.0, ErrorMessage = "Теплоемкость газа должна быть от 0.5 до 3 кДж/(м³·К)")]
        [Display(Name = "Теплоемкость газа, кДж/(м³·К)")]
        public double GasHeatCapacity { get; set; } = 1.35;
        
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(0.1, 5.0, ErrorMessage = "Расход материалов должен быть от 0.1 до 5 кг/с")]
        [Display(Name = "Расход материалов, кг/с")]
        public double MaterialFlowRate { get; set; } = 1.68;
        
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(0.5, 3.0, ErrorMessage = "Теплоемкость материалов должна быть от 0.5 до 3 кДж/(кг·К)")]
        [Display(Name = "Теплоемкость материалов, кДж/(кг·К)")]
        public double MaterialHeatCapacity { get; set; } = 1.49;
        
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(-50, 100, ErrorMessage = "Температура материала должна быть от -50 до 100 °C")]
        [Display(Name = "Начальная температура материала, °C")]
        public double InitialMaterialTemperature { get; set; } = 0;
        
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(0, 1500, ErrorMessage = "Температура газа должна быть от 0 до 1500 °C")]
        [Display(Name = "Начальная температура газа, °C")]
        public double InitialGasTemperature { get; set; } = 750;
        
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(0.1, 10.0, ErrorMessage = "Диаметр аппарата должен быть от 0.1 до 10 м")]
        [Display(Name = "Диаметр аппарата, м")]
        public double ApparatusDiameter { get; set; } = 2.2;
    }
}