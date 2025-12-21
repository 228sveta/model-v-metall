using HeatExchangeCalculator.Models;
using HeatExchangeCalculator.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HeatExchangeCalculator.Controllers
{
    public class HomeController : Controller
    {
        private readonly HeatExchangeService _heatExchangeService;
        private readonly CalculationStorageService _storageService;

        public HomeController()
        {
            _heatExchangeService = new HeatExchangeService();
            _storageService = new CalculationStorageService();
        }

        // Главная страница с формой
        public IActionResult Index()
        {
            var model = new CalculationParameters();
            return View(model);
        }

        // Обработка формы расчета
        [HttpPost]
        public IActionResult Calculate(CalculationParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", parameters);
            }

            var result = _heatExchangeService.Calculate(parameters);
            return View("Result", result);
        }

        // Страница с результатами
        public IActionResult Result()
        {
            // Если зашли напрямую - показываем пример
            var parameters = new CalculationParameters();
            var result = _heatExchangeService.Calculate(parameters);
            return View(result);
        }

        // Сохранить расчет
        [HttpPost]
        public IActionResult SaveCalculation(SaveCalculationRequest model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    model.Name = $"Расчет от {DateTime.Now:dd.MM.yyyy HH:mm}";
                }

                var savedCalculation = new SavedCalculation
                {
                    Name = model.Name,
                    Parameters = model.Result.Parameters,
                    Result = model.Result
                };

                _storageService.SaveCalculation(savedCalculation);
                
                TempData["SuccessMessage"] = $"Расчет '{model.Name}' успешно сохранен!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при сохранении: {ex.Message}";
            }
            
            return RedirectToAction("Saved");
        }

        // Загрузить расчет по ID
        public IActionResult LoadCalculation(int id)
        {
            var saved = _storageService.LoadCalculation(id);
            if (saved == null)
            {
                TempData["ErrorMessage"] = "Расчет не найден";
                return RedirectToAction("Saved");
            }
            
            return View("Result", saved.Result);
        }

        // Удалить расчет
        [HttpPost]
        public IActionResult DeleteCalculation(int id)
        {
            try
            {
                _storageService.DeleteCalculation(id);
                TempData["SuccessMessage"] = "Расчет успешно удален!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при удалении: {ex.Message}";
            }
            
            return RedirectToAction("Saved");
        }

                // Экспорт расчета в Excel
        public IActionResult ExportToExcel(int id)
        {
            try
            {
                var saved = _storageService.LoadCalculation(id);
                if (saved == null)
                {
                    TempData["ErrorMessage"] = "Расчет не найден";
                    return RedirectToAction("Saved");
                }

                // Создаем CSV с правильной кодировкой и разделителями
                var sb = new System.Text.StringBuilder();
                
                // Заголовок
                sb.AppendLine("Расчет теплообмена в плотном продуваемом слое");
                sb.AppendLine($"Название расчета: {saved.Name}");
                sb.AppendLine($"Дата сохранения: {saved.SaveDate:dd.MM.yyyy HH:mm}");
                sb.AppendLine();
                
                // Параметры
                sb.AppendLine("ИСХОДНЫЕ ПАРАМЕТРЫ");
                sb.AppendLine("Параметр;Значение;Единица измерения");
                sb.AppendLine($"Высота слоя;{saved.Parameters.LayerHeight};м");
                sb.AppendLine($"Диаметр аппарата;{saved.Parameters.ApparatusDiameter};м");
                sb.AppendLine($"Объемный коэффициент теплоотдачи;{saved.Parameters.VolumetricHeatTransferCoefficient};Вт/(м³·К)");
                sb.AppendLine($"Скорость газа;{saved.Parameters.GasVelocity};м/с");
                sb.AppendLine($"Теплоемкость газа;{saved.Parameters.GasHeatCapacity};кДж/(м³·К)");
                sb.AppendLine($"Расход материалов;{saved.Parameters.MaterialFlowRate};кг/с");
                sb.AppendLine($"Теплоемкость материалов;{saved.Parameters.MaterialHeatCapacity};кДж/(кг·К)");
                sb.AppendLine($"Начальная температура материала;{saved.Parameters.InitialMaterialTemperature};°C");
                sb.AppendLine($"Начальная температура газа;{saved.Parameters.InitialGasTemperature};°C");
                sb.AppendLine();
                
                // Расчетные величины
                sb.AppendLine("РАСЧЕТНЫЕ ВЕЛИЧИНЫ");
                sb.AppendLine("Параметр;Значение");
                sb.AppendLine($"Площадь сечения аппарата;{saved.Result.CrossSectionalArea:F3}");
                sb.AppendLine($"Отношение теплоемкостей (m);{saved.Result.HeatCapacityRatio:F3}");
                sb.AppendLine($"Полная относительная высота (Y₀);{saved.Result.TotalRelativeHeight:F3}");
                sb.AppendLine($"Знаменатель;{saved.Result.FullRelativeHeight:F4}");
                sb.AppendLine();
                
                // Результаты по точкам
                sb.AppendLine("РЕЗУЛЬТАТЫ РАСЧЕТА ПО ВЫСОТЕ СЛОЯ");
                sb.AppendLine("Высота, м;Y;θ1;θ2;ϑ;θ;t, °C;T, °C;Δt, °C");
                
                foreach (var point in saved.Result.Points)
                {
                    sb.AppendLine($"{point.Height};" +
                                $"{point.Y:F2};" +
                                $"{point.Theta1:F2};" +
                                $"{point.Theta2:F2};" +
                                $"{point.DimensionlessMaterialTemp:F2};" +
                                $"{point.DimensionlessGasTemp:F2};" +
                                $"{point.MaterialTemperature:F1};" +
                                $"{point.GasTemperature:F1};" +
                                $"{point.TemperatureDifference:F1}");
                }
                
                // Используем кодировку Windows-1251 для русского текста
                var encoding = System.Text.Encoding.GetEncoding(1251);
                var bytes = encoding.GetBytes(sb.ToString());
                
                return File(bytes, 
                    "text/csv;charset=windows-1251", 
                    $"Теплообмен_{saved.Name.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmm}.csv");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при экспорте: {ex.Message}";
                return RedirectToAction("Saved");
            }
        }

        // Страница с сохраненными расчетами
        public IActionResult Saved()
        {
            var calculations = _storageService.LoadAllCalculations();
            return View(calculations);
        }

        // Страница конфиденциальности
        public IActionResult Privacy()
        {
            return View();
        }

        // Страница ошибки
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}