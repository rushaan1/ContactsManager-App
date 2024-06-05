using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;

namespace ContactsManager_App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddScoped<IPersonService, PersonService>();
            builder.Services.AddDbContext<PersonsDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]!);
            });

            var app = builder.Build();
            Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

            app.UseRouting();
            app.UseStaticFiles();
            app.MapControllers();

            app.Run();
        }
    }
}
