using Core.Objects;
using Microsoft.EntityFrameworkCore;
using UrFUEducationalModules.Data;

namespace Core.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly ApplicationDbContext context;

    public ModuleRepository(ApplicationDbContext context)
    {
        this.context = context;
    }
    
    public IQueryable<ModuleEntity> GetModuleEntities()
    {
        return context.Modules;
    }

    public ModuleEntity GetModuleEntityById(Guid id)
    {
        return context.Modules.FirstOrDefault(t => t.Uuid == id);
    }

    public void SaveModuleEntity(ModuleEntity entity)
    {
        if (!context.Modules.Any(t => t.Uuid == entity.Uuid))
            context.Entry(entity).State = EntityState.Added;
        else
            context.Entry(entity).State = EntityState.Modified;
        context.SaveChanges();
    }

    public void DeleteModuleEntity(Guid id)
    {
        context.Modules.Remove(new ModuleEntity() { Uuid = id });
        context.SaveChanges();
    }
}