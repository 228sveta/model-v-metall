namespace HeatExchangeCalculator.Models 
{ 
    public class SaveCalculationRequest 
    { 
        public string Name { get; set; } = string.Empty; 
        public CalculationResult Result { get; set; } = new CalculationResult(); 
    } 
} 
