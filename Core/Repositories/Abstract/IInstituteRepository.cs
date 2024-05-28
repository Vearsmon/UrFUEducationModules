using Core.Objects;

namespace Core.Repositories;

public interface IInstituteRepository
{
    IQueryable<InstituteEntity> GetInstituteEntities();
    InstituteEntity GetInstituteEntityById(Guid id);
    void SaveInstituteEntity(InstituteEntity entity);
    void DeleteInstituteEntity(Guid id);
}