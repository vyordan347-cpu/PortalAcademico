using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Models;

namespace PortalAcademico.Data
{
    public static class SeedData
    {
        public static async Task InicializarAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            const string rolCoordinador = "Coordinador";

            if (!await roleManager.RoleExistsAsync(rolCoordinador))
            {
                await roleManager.CreateAsync(new IdentityRole(rolCoordinador));
            }

            const string correoCoordinador = "coordinador@portal.com";
            const string passwordCoordinador = "Coordinador123!";

            var usuario = await userManager.FindByEmailAsync(correoCoordinador);

            if (usuario == null)
            {
                usuario = new IdentityUser
                {
                    UserName = correoCoordinador,
                    Email = correoCoordinador,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(usuario, passwordCoordinador);

                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(usuario, rolCoordinador);
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(usuario, rolCoordinador))
                {
                    await userManager.AddToRoleAsync(usuario, rolCoordinador);
                }
            }

            if (!await context.Cursos.AnyAsync())
            {
                var cursos = new List<Curso>
                {
                    new Curso
                    {
                        Codigo = "MAT101",
                        Nombre = "Matemática Básica",
                        Creditos = 4,
                        CupoMaximo = 30,
                        HorarioInicio = new TimeOnly(8, 0),
                        HorarioFin = new TimeOnly(10, 0),
                        Activo = true
                    },
                    new Curso
                    {
                        Codigo = "PRO102",
                        Nombre = "Programación I",
                        Creditos = 5,
                        CupoMaximo = 25,
                        HorarioInicio = new TimeOnly(10, 30),
                        HorarioFin = new TimeOnly(12, 30),
                        Activo = true
                    },
                    new Curso
                    {
                        Codigo = "BDD201",
                        Nombre = "Base de Datos",
                        Creditos = 4,
                        CupoMaximo = 20,
                        HorarioInicio = new TimeOnly(14, 0),
                        HorarioFin = new TimeOnly(16, 0),
                        Activo = true
                    }
                };

                await context.Cursos.AddRangeAsync(cursos);
                await context.SaveChangesAsync();
            }
        }
    }
}