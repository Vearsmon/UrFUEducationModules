using Core.Objects;

namespace Core.Repositories;

public interface IProgramModuleMappingRepository
{
    IQueryable<ProgramModuleMappingEntity> GetProgramModuleMappingEntities();
    ProgramModuleMappingEntity GetProgramModuleMappingEntityById(Guid id);
    void SaveProgramModuleMappingEntity(ProgramModuleMappingEntity entity);
    void DeleteProgramModuleMappingEntity(Guid programId, Guid moduleId);
}