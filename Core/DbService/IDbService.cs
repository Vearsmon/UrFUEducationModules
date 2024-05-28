using Core.Repositories;

namespace Core;

public interface IDbService
{
    public IHeadRepository HeadsRepository { get; }
    public IInstituteRepository InstitutesRepository { get; }
    public IModuleRepository ModulesRepository { get; }
    public IProgramRepository ProgramsRepository { get; }
    public IProgramModuleMappingRepository ProgramModuleMappingsRepository { get; }
}