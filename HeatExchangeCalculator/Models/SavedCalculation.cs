namespace HeatExchangeCalculator.Models
{
    public class SavedCalculation
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime SaveDate { get; set; }
        public CalculationParameters Parameters { get; set; } = new CalculationParameters();
        public CalculationResult Result { get; set; } = new CalculationResult();
    }
}