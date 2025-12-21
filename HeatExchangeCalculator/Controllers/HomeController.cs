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
        public IActionResult SaveCalculation(string name, [FromForm] string resultJson)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = $"Расчет от {DateTime.Now:dd.MM.yyyy HH:mm}";
                }

                // Десериализуем результат из JSON
                var result = System.Text.Json.JsonSerializer.Deserialize<CalculationResult>(resultJson);
                if (result == null)
                {
                    TempData["ErrorMessage"] = "Ошибка при сохранении: неверные данные";
                    return RedirectToAction("Saved");
                }

                var savedCalculation = new SavedCalculation
                {
                    Name = name,
                    Parameters = result.Parameters,
                    Result = result
                };

                _storageService.SaveCalculation(savedCalculation);
                
                TempData["SuccessMessage"] = $"Расчет '{name}' успешно сохранен!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при сохранении: {ex.Message}";
            }
            
            return RedirectToAction("Saved");
        }

        // Загрузить расчет
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