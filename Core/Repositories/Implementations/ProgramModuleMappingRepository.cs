using Core.Objects;
using Microsoft.EntityFrameworkCore;
using UrFUEducationalModules.Data;

namespace Core.Repositories;

public class ProgramModuleMappingRepository : IProgramModuleMappingRepository
{
    private readonly ApplicationDbContext context;

    public ProgramModuleMappingRepository(ApplicationDbContext context)
    {
        this.context = context;
    }
    
    public IQueryable<ProgramModuleMappingEntity> GetProgramModuleMappingEntities()
    {
        return context.ProgramModuleMappings;
    }

    public ProgramModuleMappingEntity GetProgramModuleMappingEntityById(Guid id)
    {
        return context.ProgramModuleMappings.FirstOrDefault(t => t.ProgramUuid == id);
    }

    public void SaveProgramModuleMappingEntity(ProgramModuleMappingEntity entity)
    {
        if (!context.ProgramModuleMappings.Contains(entity))
            context.Entry(entity).State = EntityState.Added;
        else
            context.Entry(entity).State = EntityState.Modified;
        context.SaveChanges();
    }

    public void DeleteProgramModuleMappingEntity(Guid programId, Guid moduleId)
    {
        context.ProgramModuleMappings.Remove(new ProgramModuleMappingEntity() { ProgramUuid = programId, ModuleUuid = moduleId });
        context.SaveChanges();
    }
}