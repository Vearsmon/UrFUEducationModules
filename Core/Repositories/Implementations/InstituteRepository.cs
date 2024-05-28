using Core.Objects;
using Microsoft.EntityFrameworkCore;
using UrFUEducationalModules.Data;

namespace Core.Repositories;

public class InstituteRepository : IInstituteRepository
{
    private readonly ApplicationDbContext context;

    public InstituteRepository(ApplicationDbContext context)
    {
        this.context = context;
    }
    
    public IQueryable<InstituteEntity> GetInstituteEntities()
    {
        return context.Institutes;
    }

    public InstituteEntity GetInstituteEntityById(Guid id)
    {
        return context.Institutes.FirstOrDefault(t => t.Uuid == id);
    }

    public void SaveInstituteEntity(InstituteEntity entity)
    {
        if (entity.Uuid == default)
            context.Entry(entity).State = EntityState.Added;
        else
            context.Entry(entity).State = EntityState.Modified;
        context.SaveChanges();
    }

    public void DeleteInstituteEntity(Guid id)
    {
        context.Institutes.Remove(new InstituteEntity() { Uuid = id });
        context.SaveChanges();
    }
}