using Microsoft.Extensions.Hosting;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using hospital.DAO;
using hospital.Services;
using hospital.DAO.MySQL;

namespace hospital
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //DaoConfig.retrieveTest();

            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddControllersWithViews();
           
            builder.Services.AddSingleton<DAOConfig>();

            builder.Services.AddScoped<AppointmentService>();
            builder.Services.AddScoped<ScheduleService>();
            builder.Services.AddScoped<PatientService>();
            builder.Services.AddScoped<DoctorService>();
            builder.Services.AddScoped<MedicalCardService>();
            builder.Services.AddScoped<SymptomService>();
            builder.Services.AddScoped<DrugService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var databaseType = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DatabaseType"];
            if (databaseType == "MySQL")
            {

                builder.Services.AddSingleton<IPatientDAO,MySQLPatientDAO>();
                builder.Services.AddSingleton<IMedicalCardDAO,MySQLMedicalCardDAO>();
                builder.Services.AddSingleton<IAppointmentDAO,MySQLAppointmentDAO>();
                builder.Services.AddSingleton<IDoctorDAO,MySQLDoctorDAO>();
                builder.Services.AddSingleton<IPaymentDAO,MySQLPaymentDAO>();
                builder.Services.AddSingleton<IDrugDAO,MySQLDrugDAO>();
                builder.Services.AddSingleton<ISymptomDAO,MySQLSymptomDAO>();
                builder.Services.AddSingleton<IScheduleDAO,MySQLScheduleDAO>();

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
            app.UseSession();


            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Start}/{action=StartPage}");
            app.MapControllerRoute(
            name: "RecordAppointment",
            pattern: "{controller=MedicalCard}/{action=RecordAppointment}/{userId}/{userState}",
            defaults: new { controller = "MedicalCard", action = "RecordAppointment" });

           app.MapControllerRoute(
           name: "RecordAppointment",
           pattern: "{controller=MedicalCard}/{action=RecordAppointmentPrepare}/{userId}/{userState}/{appId:int}",
           defaults: new { controller = "MedicalCard", action = "RecordAppointmentPrepare" });

           



            app.Run();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();



            //services.AddScoped<DAOFactory>();



        }
    }
}
