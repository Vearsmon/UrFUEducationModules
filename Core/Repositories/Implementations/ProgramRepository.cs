using Core.Objects;
using Microsoft.EntityFrameworkCore;
using UrFUEducationalModules.Data;

namespace Core.Repositories;

public class ProgramRepository : IProgramRepository
{
    private readonly ApplicationDbContext context;

    public ProgramRepository(ApplicationDbContext context)
    {
        this.context = context;
    }
    
    public IQueryable<ProgramEntity> GetProgramEntities()
    {
        return context.Programs;
    }

    public ProgramEntity GetProgramEntityById(Guid id)
    {
        return context.Programs.FirstOrDefault(t => t.Uuid == id);
    }

    public void SaveProgramEntity(ProgramEntity entity)
    {
        if (!GetProgramEntities().Any(t => t.Uuid == entity.Uuid))
            context.Entry(entity).State = EntityState.Added;
        else
            context.Entry(entity).State = EntityState.Modified;
        context.SaveChanges();
    }

    public void DeleteProgramEntity(Guid id)
    {
        context.Programs.Remove(new ProgramEntity() { Uuid = id });
        context.SaveChanges();
    }
}