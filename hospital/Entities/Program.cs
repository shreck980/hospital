using Microsoft.Extensions.Hosting;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using hospital.DAO;

namespace hospital.Entities
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //DaoConfig.retrieveTest();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            /* builder.Services.AddScoped(s =>
             {
                 var serviceProvider = s.GetRequiredService<IServiceProvider>();
                 var dao = serviceProvider.GetService<MySQLPatientDAO>();
                 if (dao == null)
                 {
                     throw new InvalidOperationException("MySQLPatientDAO service is not available.");
                 }
                 return dao;
             });*/
            builder.Services.AddScoped<DAOConfig>();

            var databaseType = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DatabaseType"];
            if (databaseType == "MySQL")
            {

                builder.Services.AddScoped<MySQLPatientDAO>();
                builder.Services.AddScoped<MySQLMedicalCardDAO>();
                builder.Services.AddScoped<MySQLAppointmentDAO>();
               
            }
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();



            //services.AddScoped<DAOFactory>();



        }
    }
}
