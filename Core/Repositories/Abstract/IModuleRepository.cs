using Core.Objects;

namespace Core.Repositories;

public interface IModuleRepository
{
    IQueryable<ModuleEntity> GetModuleEntities();
    ModuleEntity GetModuleEntityById(Guid id);
    void SaveModuleEntity(ModuleEntity entity);
    void DeleteModuleEntity(Guid id);
}