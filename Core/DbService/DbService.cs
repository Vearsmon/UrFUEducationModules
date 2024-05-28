using Core.Repositories;
using UrFUEducationalModules.Data;

namespace Core;

public class DbService : IDbService, IDisposable, IAsyncDisposable
{
    private readonly ApplicationDbContext _context;
    
    private readonly Lazy<IHeadRepository> headsRepository;
    private readonly Lazy<IInstituteRepository> institutesRepository;
    private readonly Lazy<IModuleRepository> modulesRepository;
    private readonly Lazy<IProgramRepository> programsRepository;
    private readonly Lazy<IProgramModuleMappingRepository> programModuleMappingsRepository;

    public IHeadRepository HeadsRepository => headsRepository.Value;
    public IInstituteRepository InstitutesRepository => institutesRepository.Value;
    public IModuleRepository ModulesRepository => modulesRepository.Value;
    public IProgramRepository ProgramsRepository => programsRepository.Value;
    public IProgramModuleMappingRepository ProgramModuleMappingsRepository => programModuleMappingsRepository.Value;

    public DbService(ApplicationDbContext context)
    {
        _context = context;
        headsRepository = new Lazy<IHeadRepository>(() => new HeadRepository(context));
        institutesRepository = new Lazy<IInstituteRepository>(() => new InstituteRepository(context));
        modulesRepository = new Lazy<IModuleRepository>(() => new ModuleRepository(context));
        programsRepository = new Lazy<IProgramRepository>(() => new ProgramRepository(context));
        programModuleMappingsRepository = new Lazy<IProgramModuleMappingRepository>(() => new ProgramModuleMappingRepository(context));
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}