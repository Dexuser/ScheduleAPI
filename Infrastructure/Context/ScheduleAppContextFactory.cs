using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    // Debido a que el proyecto de infrastructure esta separado de otras capas (Onion)
    // Tengo que crear esta clase para que le inyecte una instancia de la Db con mi string connection

    // dotnet ef migrations add NombreMigracion --startup-project ../Presentation
    // Usa este codigo para agregar migraciones de ahora en adelante

    namespace Infrastructure.Context
    {

        using Microsoft.EntityFrameworkCore;
        using Microsoft.EntityFrameworkCore.Design;
        using Microsoft.Extensions.Configuration;
        using System.IO;

        namespace Infrastructure.Context
        {
            public class ScheduleAppContextFactory : IDesignTimeDbContextFactory<ScheduleAppContext>
            {
                public ScheduleAppContext CreateDbContext(string[] args)
                {
                    // Ir a la carpeta del proyecto de inicio (Presentation)
                    var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Presentation");

                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(basePath)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    var connectionString = configuration.GetConnectionString("DefaultConnection");

                    var optionsBuilder = new DbContextOptionsBuilder<ScheduleAppContext>();
                    optionsBuilder.UseSqlServer(
                        connectionString,
                        b => b.MigrationsAssembly("Infrastructure")
                    );

                    return new ScheduleAppContext(optionsBuilder.Options);
                }
            }
        }


    }


}
