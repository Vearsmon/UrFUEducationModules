using System.ComponentModel;
using Castle.Core.Internal;
using Core.Objects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UrFUEducationalModules.Data;

// Здесь создаем контекст, в котором будет работать БД. Тут же создаем начальные данные для работы.
// Все сущности БД, отображающиеся в классы, описаны в классах, которые оформлены как дженерики в строках 15-19.
// В директории Repositories описаны все сервисы, реализующие CRUD операции в приложении. Именно через них
// происходит все взаимодействие с БД. Также есть миграции, на всякий случай, чтобы при краше базы не потерять структуру
// и данные. DbService и DbServiceProvider осуществляют передачу контекста БД в разные методы контроллеров, чтобы
// централизованно управлять всеми репозиториями и не привязывать сильно контроллеры к БД (оставляем только один
// эндпоинт к БД в виде сервиса

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<HeadEntity> Heads { get; set; } = null!;
    public DbSet<InstituteEntity> Institutes { get; set; } = null!;
    public DbSet<ModuleEntity> Modules { get; set; } = null!;
    public DbSet<ProgramEntity> Programs { get; set; } = null!;
    public DbSet<ProgramModuleMappingEntity> ProgramModuleMappings { get; set; } = null!;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole()
        {
            Id = "4e5c0dd9-d041-4a52-a449-fa69f6184bbe",
            Name = "admin",
            NormalizedName = "ADMIN"
        });
        
        modelBuilder.Entity<IdentityUser>().HasData(new IdentityUser()
        {
            Id = "2f775ba4-4f49-414c-8d7c-a78409f3639d",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@email.com",
            NormalizedEmail = "ADMIN@EMAIL.COM",
            EmailConfirmed = true,
            PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, "qwerty123"),
            SecurityStamp = ""
        });

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>()
        {
            RoleId = "4e5c0dd9-d041-4a52-a449-fa69f6184bbe",
            UserId = "2f775ba4-4f49-414c-8d7c-a78409f3639d"
        });

        modelBuilder.Entity<ProgramEntity>().HasData(new ProgramEntity()
        {
            Uuid = new Guid("30a12709-e029-400d-85a4-582109fbb923"),
            Title = "Разработка программно-информационных систем",
            Status = "Действующая до завершения срока освоения",
            Cypher = "09.04.03/33.03",
            Level = Level.Master,
            Standard = Standard.SUOS,
            Institute = new Guid("6a0099be-2bd4-409b-aee2-10984c4df380"),
            Head = new Guid("0e887f76-2a05-11e1-b174-00259030b74f"),
            AccreditationTime = new DateTime(2025, 3, 14).ToUniversalTime()
        });

        modelBuilder.Entity<ProgramEntity>().HasKey("Uuid");

        modelBuilder.Entity<InstituteEntity>().HasData(new InstituteEntity()
        {
            Uuid = new Guid("6a0099be-2bd4-409b-aee2-10984c4df380"),
            Title = "Институт радиоэлектроники и информационных технологий - РтФ"
        });
        
        modelBuilder.Entity<InstituteEntity>().HasData(new InstituteEntity()
        {
            Uuid = new Guid("41d6b9cb-bfc0-4619-9d01-9125f407d458"),
            Title = "Институт новых материалов и технологий - ИНМТ"
        });
        
        modelBuilder.Entity<InstituteEntity>().HasData(new InstituteEntity()
        {
            Uuid = new Guid("a71450fb-ad84-4d75-b226-25ce4a2f1648"),
            Title = "Институт естественных наук и математики - ИЕНиМ"
        });
        
        modelBuilder.Entity<InstituteEntity>().HasKey("Uuid");

        modelBuilder.Entity<HeadEntity>().HasData(new HeadEntity()
        {
            Uuid = new Guid("0e887f76-2a05-11e1-b174-00259030b74f"),
            Fullname = "Обухов Олег Владимирович"
        });
        
        modelBuilder.Entity<HeadEntity>().HasData(new HeadEntity()
        {
            Uuid = new Guid("8ff4c2c6-9d6b-44fd-93bc-550dd2f154a2"),
            Fullname = "Овчинников Максим Михайлович"
        });
        
        modelBuilder.Entity<HeadEntity>().HasData(new HeadEntity()
        {
            Uuid = new Guid("62850c3d-e440-4969-9456-8dfe9c329f96"),
            Fullname = "Кузьмин Федор Петрович"
        });
        
        modelBuilder.Entity<HeadEntity>().HasKey("Uuid");

        modelBuilder.Entity<ModuleEntity>().HasData(new ModuleEntity()
        {
            Uuid = new Guid("54e561d2-0a27-4644-b67a-47ab4e7881bc"),
            Title = "Растровая графика",
            Type = "STANDARD"
        });
        
        modelBuilder.Entity<ModuleEntity>().HasData(new ModuleEntity()
        {
            Uuid = new Guid("ad25a898-4364-4a1a-b9df-b10d68c3a093"),
            Title = "Векторная графика",
            Type = "STANDARD"
        });
        
        modelBuilder.Entity<ModuleEntity>().HasData(new ModuleEntity()
        {
            Uuid = new Guid("b2b666ed-728b-4bd1-8840-72eeda5a3a31"),
            Title = "Психология предпринимательства",
            Type = "MINOR"
        });
        
        modelBuilder.Entity<ModuleEntity>().HasData(new ModuleEntity()
        {
            Uuid = new Guid("a50eefaa-d67c-4917-a143-c194ec1b9929"),
            Title = "Основы астрономии",
            Type = "PROJECT"
        });
        
        modelBuilder.Entity<ModuleEntity>().HasKey("Uuid");

        modelBuilder.Entity<ProgramModuleMappingEntity>().HasData(new ProgramModuleMappingEntity()
        {
            ProgramUuid = new Guid("30a12709-e029-400d-85a4-582109fbb923"),
            ModuleUuid = new Guid("54e561d2-0a27-4644-b67a-47ab4e7881bc")
        });
        
        modelBuilder.Entity<ProgramModuleMappingEntity>().HasData(new ProgramModuleMappingEntity()
        {
            ProgramUuid = new Guid("30a12709-e029-400d-85a4-582109fbb923"),
            ModuleUuid = new Guid("ad25a898-4364-4a1a-b9df-b10d68c3a093")
        });

        modelBuilder.Entity<ProgramModuleMappingEntity>().HasKey(pm => new { pm.ProgramUuid, pm.ModuleUuid });
    }
}