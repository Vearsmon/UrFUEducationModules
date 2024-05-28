using System.Diagnostics;
using Core;
using Core.Objects;
using Microsoft.AspNetCore.Mvc;
using UrFUEducationalModules.Models;

namespace UrFUEducationalModules.Controllers;

// Базовый контроллер, который отображает не только страницы с контентом, но и страницы авторизации пользователя.
// Решил так сделать, потому что подумал, что два контроллера на несколько страничек будет много.
// Этого контроллера и API хватает для решения задачи.
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDbService _dbService;

    public HomeController(ILogger<HomeController> logger, IDbService dbService)
    {
        _logger = logger;
        _dbService = dbService;
    }

    public IActionResult Index()
    {
        var programs = _dbService.ProgramsRepository.GetProgramEntities();
        return View(programs);
    }

    public IActionResult Modules()
    {
        var modules = _dbService.ModulesRepository.GetModuleEntities();
        return View(modules);
    }

    public IActionResult Register()
    {
        return View();
    }
    
    public IActionResult Login()
    {
        return View();
    }
}