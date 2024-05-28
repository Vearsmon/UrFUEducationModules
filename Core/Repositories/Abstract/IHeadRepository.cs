using Core.Objects;

namespace Core.Repositories;

public interface IHeadRepository
{
    IQueryable<HeadEntity> GetHeadEntities();
    HeadEntity GetHeadEntityById(Guid id);
    void SaveHeadEntity(HeadEntity entity);
    void DeleteHeadEntity(Guid id);
}