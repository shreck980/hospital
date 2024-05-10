using hospital.DAO;
using hospital.Entities;
using hospital.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace hospital.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MySQLPatientDAO _patientDAO;
        private readonly MySQLMedicalCardDAO _medicalCardDAO;
        private readonly MySQLAppointmentDAO _appointmentDAO;
        private readonly MySQLDoctorDAO _doctorDAO;
        private readonly MySQLPaymentDAO _paymentDAO;

        private Patient example = new Patient("Susan", "Lubowski", "susan556@gmail.com", "ilovecats234", new DateTime(1999, 8, 12), "Wrozlaw, Beautiful street,55");
        Patient example2 = new Patient(
         "John",
         "Doe",
         "johndoe@example.com",
         "securepass123",
         new DateTime(1985, 5, 20),
         "123 Main Street, Anytown, USA"
            );
        public HomeController(ILogger<HomeController> logger,MySQLPatientDAO mySQLPatientDAO, MySQLMedicalCardDAO medicalCardDAO)
        {
            _logger = logger;
            _patientDAO = mySQLPatientDAO;
            _medicalCardDAO = medicalCardDAO;
        }

        public IActionResult Index()
        {
            //_patientDAO.GetAllPatients();
            //_patientDAO.GetAllPatients();
           // _patientDAO.AddPatientWithout—onfirmation(example2);
            return View();
        }

        public IActionResult Privacy()
        {
            
           /* example2 = _patientDAO.GetPatientByEmail(example2.Email);
            example2.MedicalCard = new MedicalCard();
            example2.State = AccountStates.Verified;
            _patientDAO.UpdatePatientWith—onfirmation(example2 );
            _medicalCardDAO.AddMedicalCard(example2.MedicalCard,example2.Id);*/
            //_patientDAO.AddPatientWith—onfirmation(example2, example2.MedicalCard);
            //_patientDAO.GetAllPatients();

            Appointment a = new Appointment();
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
