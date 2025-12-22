using HeatExchangeCalculator.Models;
using HeatExchangeCalculator.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HeatExchangeCalculator.Controllers
{
    public class HomeController : Controller
    {
        private readonly HeatExchangeService _heatExchangeService;
        private readonly SimpleDbService _dbService;

        public HomeController(
            HeatExchangeService heatExchangeService,
            SimpleDbService dbService)
        {
            _heatExchangeService = heatExchangeService;
            _dbService = dbService;
        }

        // Главная страница
        public IActionResult Index()
        {
            var model = new CalculationParameters();
            return View(model);
        }

        // Расчет
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

        // Сохранить расчет
        [HttpPost]
        public IActionResult SaveCalculation(string name, CalculationResult result)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = $"Расчет от {DateTime.Now:dd.MM.yyyy HH:mm}";
                }

                _dbService.SaveCalculation(name, result);
                TempData["SuccessMessage"] = $"Расчет '{name}' сохранен!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToAction("Saved");
        }

        // Загрузить расчет
        public IActionResult LoadCalculation(int id)
        {
            var saved = _dbService.LoadCalculation(id);
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
                _dbService.DeleteCalculation(id);
                TempData["SuccessMessage"] = "Расчет удален!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToAction("Saved");
        }

        // Просмотр сохраненных
        public IActionResult Saved()
        {
            var calculations = _dbService.LoadAllCalculations();
            return View(calculations);
        }

        // Страница результата
        public IActionResult Result()
        {
            var parameters = new CalculationParameters();
            var result = _heatExchangeService.Calculate(parameters);
            return View(result);
        }

        // Конфиденциальность
        public IActionResult Privacy()
        {
            return View();
        }

        // Ошибка
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}