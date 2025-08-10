using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Context;

// Debido a que el proyecto de infrastructure esta separado de otras capas (Onion)
// Tengo que crear esta clase para que le inyecte una instancia de la Db con mi string connection

// dotnet ef migrations add NombreMigracion --startup-project ../Presentation
// Usa este codigo para agregar migraciones de ahora en adelante
public class ScheduleAppContextFactory : IDesignTimeDbContextFactory<ScheduleAppContext>
{
    public ScheduleAppContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ScheduleAppContext>();
        optionsBuilder.UseSqlServer(
            "Server=LAPTOP-T7P92N9J; Database=ScheduleApp; Trusted_Connection=True; TrustServerCertificate=True;", // usa tu cadena real
            b => b.MigrationsAssembly("Infrastructure")); // asegura que las migraciones se generen aquí

        return new ScheduleAppContext(optionsBuilder.Options);
    }
}