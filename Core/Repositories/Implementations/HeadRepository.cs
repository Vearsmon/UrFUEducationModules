using Core.Objects;
using Microsoft.EntityFrameworkCore;
using UrFUEducationalModules.Data;

namespace Core.Repositories;

public class HeadRepository : IHeadRepository
{
    private readonly ApplicationDbContext context;

    public HeadRepository(ApplicationDbContext context)
    {
        this.context = context;
    }
    
    public IQueryable<HeadEntity> GetHeadEntities()
    {
        return context.Heads;
    }

    public HeadEntity GetHeadEntityById(Guid id)
    {
        return context.Heads.FirstOrDefault(t => t.Uuid == id);
    }

    public void SaveHeadEntity(HeadEntity entity)
    {
        if (entity.Uuid == default)
            context.Entry(entity).State = EntityState.Added;
        else
            context.Entry(entity).State = EntityState.Modified;
        context.SaveChanges();
    }

    public void DeleteHeadEntity(Guid id)
    {
        context.Heads.Remove(new HeadEntity() { Uuid = id });
        context.SaveChanges();
    }
}