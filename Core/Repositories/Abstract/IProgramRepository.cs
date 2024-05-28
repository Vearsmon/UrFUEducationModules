using Core.Objects;

namespace Core.Repositories;

public interface IProgramRepository
{
    IQueryable<ProgramEntity> GetProgramEntities();
    ProgramEntity GetProgramEntityById(Guid id);
    void SaveProgramEntity(ProgramEntity entity);
    void DeleteProgramEntity(Guid id);
}