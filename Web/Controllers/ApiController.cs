using Core; // Импорт, содержащий всевозможные сервисы и "ручки" для БД
using Core.Objects; // Непосредственно сущности БД
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrFUEducationalModules.Models;

namespace UrFUEducationalModules.Controllers;

[Route("api")]
public class ApiController : Controller
{
    private readonly UserManager<IdentityUser> _userManager; // управляет пользователями приложения
    private readonly SignInManager<IdentityUser> _signInManager; // управляет входом пользователей
    private readonly IDbServiceProvider _dbServiceProvider;
    public ApiController(
        UserManager<IdentityUser> userMgr, 
        SignInManager<IdentityUser> signinMgr,
        IDbServiceProvider dbServiceProvider)
    {
        // Здесь происходит инджект основных сервисов для работы API
        _userManager = userMgr;
        _signInManager = signinMgr;
        _dbServiceProvider = dbServiceProvider; // нужен для правильной работы с concurrency в БД и смежных сервисов
    }
    
    [HttpGet]
    [Route("isAuthenticated")]
    public JsonResult IsAuthenticated()
    {
        var isSigned = _signInManager.IsSignedIn(User);
        return isSigned ? Json(User.Identity?.Name) : Json(isSigned);
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                if (result.Succeeded)
                {
                    return Redirect("/");
                }
            }
            ModelState.AddModelError(nameof(LoginViewModel.UserName), "Неверный логин или пароль");
        }
        return Redirect($"/Home/Login");
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if(ModelState.IsValid)
        {
            var user = new IdentityUser() { Email = model.UserName + "@email.com", UserName = model.UserName };
            if (model.Password == model.PasswordConfirm)
            {
                var result = await _userManager.CreateAsync(user, model.Password); // пароль идеально хранить в хешированном виде, однако требуется лишь базовая авторизация
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return Redirect($"/Home/Index");
    }

    [HttpGet]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("get/program")] // по Guid получает ОП. Стандартная модель ОП заменяется на модель с читаемыми именами
    public JsonResult GetProgram()
    {
        var uuid = HttpContext.Request.QueryString.ToString()[1..^1];
        // получаем отдельные экземпляры соединений с БД (имитация пула соединений)
        var program = _dbServiceProvider.Get().ProgramsRepository.GetProgramEntityById(new Guid(uuid));
        var institute = _dbServiceProvider.Get().InstitutesRepository.GetInstituteEntityById(program.Institute).Title;
        var head = _dbServiceProvider.Get().HeadsRepository.GetHeadEntityById(program.Head).Fullname;
        var programInfo = new
        {
            Uuid = program.Uuid,
            Title = program.Title,
            Status = program.Status,
            Cypher = program.Cypher,
            Level = program.Level,
            Standard = program.Standard,
            Institute = institute,
            Head = head,
            AccreditationTime = program.AccreditationTime,
        };
        return Json(programInfo);
    }
    
    [HttpGet]
    [Route("get/module")] // Учебный модуль по Guid
    public JsonResult GetModule()
    {
        var uuid = HttpContext.Request.QueryString.ToString()[1..^1]; // по какой-то причине Guid с фронта идут с лишними символами, обрезаем
        var module = _dbServiceProvider.Get().ModulesRepository.GetModuleEntityById(new Guid(uuid));
        return Json(module);
    }

    [HttpGet]
    [Route("get/levels")] // Список всех уровней обучения. Достаем через рефлексию, поскольку enum'а было недостаточно для связки имен с кодовыми названиями
    public JsonResult GetLevels()
    {
        var levels = typeof(Level).GetFields().Select(f => f.GetValue(null)!.ToString());
        return Json(levels);
    }
    
    [HttpGet]
    [Route("get/standards")] // Список всех стандартов обучения. Такая же рефлексия
    public JsonResult GetStandards()
    {
        var standards = typeof(Standard).GetFields().Select(f => f.GetValue(null)!.ToString());
        return Json(standards);
    }
    
    [HttpGet]
    [Route("get/institutes")] // Список всех институтов
    public JsonResult GetInstitutes()
    {
        var institutes = _dbServiceProvider.Get().InstitutesRepository.GetInstituteEntities().Select(t => t.Title);
        return Json(institutes);
    }
    
    [HttpGet]
    [Route("get/institute/title")] // Получить по Guid имя института
    public JsonResult GetInstituteTitle()
    {
        var uuid = new Guid(HttpContext.Request.QueryString.ToString()[1..^1]);
        var instituteTitle = _dbServiceProvider.Get().InstitutesRepository.GetInstituteEntityById(uuid).Title;
        return Json(instituteTitle);
    }
    
    [HttpGet]
    [Route("get/head/fullname")] // По Guid достаем ФИО ответственного лица
    public JsonResult GetHeadFullname()
    {
        var uuid = new Guid(HttpContext.Request.QueryString.ToString()[1..^1]);
        var headFullname = _dbServiceProvider.Get().HeadsRepository.GetHeadEntityById(uuid).Fullname;
        return Json(headFullname);
    }
    
    [HttpGet]
    [Route("get/heads")] // Список всех ответственных лиц
    public JsonResult GetHeads()
    {
        var heads = _dbServiceProvider.Get().HeadsRepository.GetHeadEntities().Select(t => t.Fullname);
        return Json(heads);
    }
    
    [HttpGet]
    [Route("get/modules")] // Список всех учебных модулей
    public JsonResult GetModules()
    {
        var modules = _dbServiceProvider.Get().ModulesRepository.GetModuleEntities().Select(t => t.Title);
        return Json(modules);
    }
    
    [HttpGet]
    [Route("get/linkedModules")] // Список всех учебных модулей, привязанных к определенной ОП (для отображения в деталях программы)
    public JsonResult GetLinkedModules()
    {
        var uuid = new Guid(HttpContext.Request.QueryString.ToString()[1..^1]);
        var modulesUuids = _dbServiceProvider.Get().ProgramModuleMappingsRepository.GetProgramModuleMappingEntities()
            .Where(t => t.ProgramUuid == uuid).Select(t => t.ModuleUuid);
        var modules = _dbServiceProvider.Get().ModulesRepository.GetModuleEntities();
        var titles = modulesUuids.Select(t => modules.FirstOrDefault(x => x.Uuid == t)!.Title);
        return Json(titles);
    }
    
    [HttpPost]
    [Route("delete/program")] // Реализуем удаление программы из БД и из всех связок с модулями (больше программа не трекается нигде)
    public JsonResult DeleteProgram()
    {
        var uuid = new Guid(HttpContext.Request.QueryString.ToString()[1..^1]);
        _dbServiceProvider.Get().ProgramsRepository.DeleteProgramEntity(uuid);
        var mappings = _dbServiceProvider.Get().ProgramModuleMappingsRepository
            .GetProgramModuleMappingEntities()
            .Where(t => t.ProgramUuid == uuid)
            .AsNoTracking()
            .ToList();
        foreach (var map in mappings)
            _dbServiceProvider.Get().ProgramModuleMappingsRepository.DeleteProgramModuleMappingEntity(map.ProgramUuid, map.ModuleUuid);
        return Json(Ok());
    }
    
    [HttpPost]
    [Route("delete/module")] // Удаляем модуль из БД и всех связок
    public JsonResult DeleteModule()
    {
        var uuid = new Guid(HttpContext.Request.QueryString.ToString()[1..^1]);
        _dbServiceProvider.Get().ModulesRepository.DeleteModuleEntity(uuid);
        var mappings = _dbServiceProvider.Get().ProgramModuleMappingsRepository
            .GetProgramModuleMappingEntities()
            .AsNoTracking()
            .Where(t => t.ModuleUuid == uuid)
            .ToList();
        foreach (var map in mappings)
            _dbServiceProvider.Get().ProgramModuleMappingsRepository.DeleteProgramModuleMappingEntity(map.ProgramUuid, map.ModuleUuid);
        return Json(Ok());
    }
    
    [HttpPost]
    [Route("save/program")] // Сохраняем ОП в БД, если она новая или ее изменили, перетираем старую
    public async Task<JsonResult> SaveProgram()
    {
        // здесь может быть очень много потоков соединений, которые будут конкурировать между собой.
        // Поэтому сделаем сохранение помедленнее (с одним подключением), но зато безопаснее
        using (var dbContext = _dbServiceProvider.Get())
        {
            var form = await HttpContext.Request.ReadFormAsync();

            // Извлекаем все данные из текстового представления на фронте и конвертируем их в Guid
            var institute = dbContext.InstitutesRepository
                .GetInstituteEntities().FirstOrDefault(t => t.Title == form["institute"].ToString())!.Uuid;
            var head = dbContext.HeadsRepository.GetHeadEntities()
                .FirstOrDefault(t => t.Fullname == form["head"].ToString())!.Uuid;
            var program = new ProgramEntity()
            {
                Uuid = form["uuid"].ToString().Length == 36 ? new Guid(form["uuid"].ToString()) : Guid.NewGuid(),
                Title = form["title"].ToString(),
                Status = form["status"].ToString(),
                Cypher = form["cypher"].ToString(),
                Level = form["level"].ToString(),
                Standard = form["standard"].ToString(),
                Institute = institute,
                Head = head,
                AccreditationTime = DateTime.Parse(form["accreditationTime"].ToString())
            };
            
            dbContext.ProgramsRepository.SaveProgramEntity(program); // Сохраняем программу перед тем, как строить для нее связи с модулями
            
            // Чистим все маппинги, чтобы заново заполнить БД только актуальными связями
            var mappings = dbContext.ProgramModuleMappingsRepository
                .GetProgramModuleMappingEntities()
                .AsNoTracking()
                .Where(t => t.ProgramUuid == program.Uuid)
                .ToList();
            foreach (var map in mappings)
                dbContext.ProgramModuleMappingsRepository.DeleteProgramModuleMappingEntity(map.ProgramUuid, map.ModuleUuid);

            var modules = form["modules"].ToString().Split('@');
        
            // Сохраняем связь с каждым актуальным маппингом
            foreach (var module in modules)
            {
                if (module == "")
                    continue;
                var allModules = dbContext.ModulesRepository.GetModuleEntities();
                var moduleUuid = allModules.FirstOrDefault(t => t.Title == module)!.Uuid;
                var mapping = new ProgramModuleMappingEntity()
                {
                    ProgramUuid = program.Uuid,
                    ModuleUuid = moduleUuid
                };
            
                dbContext.ProgramModuleMappingsRepository.SaveProgramModuleMappingEntity(mapping);
            }
            
            return Json(program);
        }
    }

    [HttpPost]
    [Route("save/module")] // Сохраняем модуль, вытаскивая данные из пришедшей FormData
    public async Task<JsonResult> SaveModule()
    {
        var form = await HttpContext.Request.ReadFormAsync();
        
        var module = new ModuleEntity()
        {
            Uuid = form["module-uuid"].ToString().Length == 36 ? new Guid(form["module-uuid"].ToString()) : Guid.NewGuid(),
            Title = form["module-title"].ToString(),
            Type = form["module-type"].ToString()
        };
        
        _dbServiceProvider.Get().ModulesRepository.SaveModuleEntity(module);
        
        return Json(module);
    }
}