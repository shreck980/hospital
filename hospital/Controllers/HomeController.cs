using hospital.DAO;
using hospital.DAO.MySQL;
using hospital.Entities;
using hospital.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Numerics;

namespace hospital.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        /* private readonly MySQLPatientDAO _patientDAO;
         private readonly MySQLMedicalCardDAO _medicalCardDAO;
         private readonly MySQLAppointmentDAO _appointmentDAO;
         private readonly MySQLDoctorDAO _doctorDAO;
         private readonly MySQLPaymentDAO _paymentDAO;
         private readonly MySQLSymptomDAO _symptomDAO;
         private readonly MySQLDrugDAO _drugDAO;
         private readonly MySQLScheduleDAO _scheduleDAO;*/
        private readonly IScheduleDAO _scheduleDAO;
        private readonly IDoctorDAO _doctorDAO;

        private Patient example = new Patient("Susan", "Lubowski", "susan556@gmail.com", "ilovecats234", new DateTime(1999, 8, 12), "Wrozlaw, Beautiful street,55");
        Patient example2 = new Patient(
         "John",
         "Doe",
         "johndoe@example.com",
         "securepass123",
         new DateTime(1985, 5, 20),
         "123 Main Street, Anytown, USA"
            );

       // Doctor doctor1 = new Doctor("Martin", "Kyil", "kyil888@example.com", "password123", AccountStates.Active,new List<Event>());
        Doctor doctor2 = new Doctor("����", "�������", "semenko555@example.com", "555", AccountStates.Active, new List<Event> { 
            new Event(new DateTime(2024,05,22,13,0,0), new DateTime(2024, 05, 22, 13, 30, 0),null),    
            new Event(new DateTime(2024,05,22,13,30,0), new DateTime(2024, 05, 22, 14, 00, 0),null), 
            new Event(new DateTime(2024,05,22,14,00,0), new DateTime(2024, 05, 22, 14, 30, 0),null) },
            Speciality.Ophthalmologist);

        private Appointment exampleA = new Appointment();
        DateTime appointmentTime;
        public HomeController(ILogger<HomeController> logger,IDoctorDAO doctorDAO, IScheduleDAO scheduleDAO/*,MySQLPatientDAO mySQLPatientDAO, MySQLMedicalCardDAO medicalCardDAO,
            MySQLAppointmentDAO appointmentDAO,MySQLDoctorDAO doctorDAO,MySQLPaymentDAO paymentDAO, MySQLDrugDAO drugDAO, MySQLSymptomDAO symptomDAO,MySQLScheduleDAO scheduleDAO)*/ )
        {
            _logger = logger;
            _doctorDAO = doctorDAO;
            _scheduleDAO = scheduleDAO;
            /*_patientDAO = mySQLPatientDAO;
            _medicalCardDAO = medicalCardDAO;
            _appointmentDAO = appointmentDAO;
            _doctorDAO = doctorDAO;
            _paymentDAO = paymentDAO;
            _drugDAO = drugDAO;
            _symptomDAO = symptomDAO;
            _scheduleDAO = scheduleDAO;*/

        }

        public IActionResult Index()
        {
            Doctor doctor = new Doctor { 
                Name="�����",
                Surname = "�������",
                Email="perlyna782@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("555"),
                State=AccountStates.Active,
                Schedule = new List<Event>{
                    new Event(new DateTime(2024, 06, 3, 13, 0, 0), new DateTime(2024, 06, 3, 13, 30, 0), null),
                    new Event(new DateTime(2024, 06, 3, 13, 30, 0), new DateTime(2024, 06, 3, 14, 00, 0), null),
                    new Event(new DateTime(2024, 06, 7, 14, 00, 0), new DateTime(2024, 06, 7, 14, 30, 0), null)
                },

                Speciality = Speciality.Gynecologist 

             };

            Doctor doctor2 = new Doctor
            {
                Name = "�����",
                Surname = "�������",
                Email = "velychko123@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("555"),
                State = AccountStates.Active,
                Schedule = new List<Event>{
        new Event(new DateTime(2024, 06, 4, 09, 0, 0), new DateTime(2024, 06, 4, 09, 30, 0), null),
        new Event(new DateTime(2024, 06, 4, 10, 0, 0), new DateTime(2024, 06, 4, 10, 30, 0), null),
        new Event(new DateTime(2024, 06, 8, 11, 0, 0), new DateTime(2024, 06, 8, 11, 30, 0), null)
    },
                Speciality = Speciality.Gastroenterologist
            };

            Doctor doctor3 = new Doctor
            {
                Name = "����",
                Surname = "�����",
                Email = "dzura@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("555"),
                State = AccountStates.Active,
                Schedule = new List<Event>{
        new Event(new DateTime(2024, 06, 5, 15, 0, 0), new DateTime(2024, 06, 5, 15, 30, 0), null),
        new Event(new DateTime(2024, 06, 5, 16, 0, 0), new DateTime(2024, 06, 5, 16, 30, 0), null),
        new Event(new DateTime(2024, 06, 9, 17, 0, 0), new DateTime(2024, 06, 9, 17, 30, 0), null)
    },
                Speciality = Speciality.Dermatologist
            };

            Doctor doctor4 = new Doctor
            {
                Name = "������",
                Surname = "�����",
                Email = "flower666@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("555"),
                State = AccountStates.Active,
                Schedule = new List<Event>{
        new Event(new DateTime(2024, 06, 6, 08, 0, 0), new DateTime(2024, 06, 6, 08, 30, 0), null),
        new Event(new DateTime(2024, 06, 6, 09, 0, 0), new DateTime(2024, 06, 6, 09, 30, 0), null),
        new Event(new DateTime(2024, 06, 10, 10, 0, 0), new DateTime(2024, 06, 10, 10, 30, 0), null)
    },
                Speciality = Speciality.Cardiologist
            };

            Doctor doctor5 = new Doctor
            {
                Name = "������",
                Surname = "��������",
                Email = "volonyc88@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("555"),
                State = AccountStates.Active,
                Schedule = new List<Event>{
        new Event(new DateTime(2024, 06, 7, 12, 0, 0), new DateTime(2024, 06, 7, 12, 30, 0), null),
        new Event(new DateTime(2024, 06, 7, 13, 0, 0), new DateTime(2024, 06, 7, 13, 30, 0), null),
        new Event(new DateTime(2024, 06, 11, 14, 0, 0), new DateTime(2024, 06, 11, 14, 30, 0), null)
    },
                Speciality = Speciality.Neurologist
            };

            Doctor doctor1= new Doctor
            {
                Name = "�����",
                Surname = "ϳ�������",
                Email = "o.pirnykoza@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("555"),
                State = AccountStates.Active,
                Schedule = new List<Event>{
        new Event(new DateTime(2024, 06, 8, 10, 0, 0), new DateTime(2024, 06, 8, 10, 30, 0), null),
        new Event(new DateTime(2024, 06, 8, 11, 0, 0), new DateTime(2024, 06, 8, 11, 30, 0), null),
        new Event(new DateTime(2024, 06, 12, 12, 0, 0), new DateTime(2024, 06, 12, 12, 30, 0), null)
    },
                Speciality = Speciality.Dentist
            };

           /* _scheduleDAO.AddSchedule(new List<Event>{
                    new Event(new DateTime(2024, 06, 3, 13, 0, 0), new DateTime(2024, 06, 3, 13, 30, 0), null),
                    new Event(new DateTime(2024, 06, 3, 13, 30, 0), new DateTime(2024, 06, 3, 14, 00, 0), null),
                    new Event(new DateTime(2024, 06, 7, 14, 00, 0), new DateTime(2024, 06, 7, 14, 30, 0), null)
                }, 5);
            _scheduleDAO.AddSchedule(new List<Event>{
        new Event(new DateTime(2024, 06, 8, 10, 0, 0), new DateTime(2024, 06, 8, 10, 30, 0), null),
        new Event(new DateTime(2024, 06, 8, 11, 0, 0), new DateTime(2024, 06, 8, 11, 30, 0), null),
        new Event(new DateTime(2024, 06, 12, 12, 0, 0), new DateTime(2024, 06, 12, 12, 30, 0), null)
    }, 6);

            _scheduleDAO.AddSchedule(new List<Event>{
        new Event(new DateTime(2024, 06, 7, 12, 0, 0), new DateTime(2024, 06, 7, 12, 30, 0), null),
        new Event(new DateTime(2024, 06, 7, 13, 0, 0), new DateTime(2024, 06, 7, 13, 30, 0), null),
        new Event(new DateTime(2024, 06, 11, 14, 0, 0), new DateTime(2024, 06, 11, 14, 30, 0), null)
    }, 7);
            _scheduleDAO.AddSchedule(new List<Event>{
        new Event(new DateTime(2024, 06, 6, 08, 0, 0), new DateTime(2024, 06, 6, 08, 30, 0), null),
        new Event(new DateTime(2024, 06, 6, 09, 0, 0), new DateTime(2024, 06, 6, 09, 30, 0), null),
        new Event(new DateTime(2024, 06, 10, 10, 0, 0), new DateTime(2024, 06, 10, 10, 30, 0), null)
    }, 8);

            _scheduleDAO.AddSchedule(new List<Event>{
        new Event(new DateTime(2024, 06, 4, 09, 0, 0), new DateTime(2024, 06, 4, 09, 30, 0), null),
        new Event(new DateTime(2024, 06, 4, 10, 0, 0), new DateTime(2024, 06, 4, 10, 30, 0), null),
        new Event(new DateTime(2024, 06, 8, 11, 0, 0), new DateTime(2024, 06, 8, 11, 30, 0), null)
    }, 9);

            _scheduleDAO.AddSchedule(new List<Event>{
        new Event(new DateTime(2024, 06, 5, 15, 0, 0), new DateTime(2024, 06, 5, 15, 30, 0), null),
        new Event(new DateTime(2024, 06, 5, 16, 0, 0), new DateTime(2024, 06, 5, 16, 30, 0), null),
        new Event(new DateTime(2024, 06, 9, 17, 0, 0), new DateTime(2024, 06, 9, 17, 30, 0), null)
    }, 10);*/


            //_patientDAO.GetAllPatients();
            //_patientDAO.GetAllPatients();
            // _patientDAO.AddPatientWithout�onfirmation(example2);

            /* appointmentTime = DateTime.Now.AddDays(7);
            Patient p = _patientDAO.GetPatientById(2);
            Doctor d = _doctorDAO.GetDoctorById(1);
            Payment payment = new Payment(1, 100.00m, DateTime.Now, DateTime.Now.AddDays(1),p);

           exampleA = new Appointment( p, d, appointmentTime, "Checkup", AppointmentState.Planned, 101, payment);
            _appointmentDAO.AddApppointment(exampleA);*/

            return View();
        }

        public IActionResult Privacy()
        {
            //string passwordHash = BCrypt.Net.BCrypt.HashPassword("111");
            //bool valid = BCrypt.Net.BCrypt.Verify("111", passwordHash);
            //_patientDAO.AddPatientWithout�onfirmation(example);
            //_doctorDAO.AddDoctor(doctor1);
            /* example2 = _patientDAO.GetPatientByEmail(example2.Email);
             example2.MedicalCard = new MedicalCard();
             example2.State = AccountStates.Verified;
             _patientDAO.UpdatePatientWith�onfirmation(example2 );
             _medicalCardDAO.AddMedicalCard(example2.MedicalCard,example2.Id);*/
            //_patientDAO.AddPatientWith�onfirmation(example2, example2.MedicalCard);
            //_patientDAO.GetAllPatients();
            /*Patient p = _patientDAO.GetPatientById(2);
            Appointment? a = _appointmentDAO.GetAppointmentByPatientAndTime(p, appointmentTime);
            if(a== null)
            {
                throw new Exception("Appointment is null:(");
            }
            a.Patient = p;
            a.Doctor =_doctorDAO.GetDoctorById(a.Doctor.Id);
            var newPayment = _paymentDAO.GetPaymentById(a.Payment.Id);
            if (newPayment != null)
            {
                a.Payment = newPayment;
            }
            a.Payment.Patient=_patientDAO.GetPatientById(a.Payment.Patient.Id);
            Console.WriteLine(a);*/
            /* appointmentTime = DateTime.Now;
             Patient p = _patientDAO.GetPatientById(1);
             Doctor d = _doctorDAO.GetDoctorById(1);
             Payment payment = new Payment(1, 100.00m, DateTime.Now, p);
             exampleA = new Appointment(p, d, appointmentTime, "Checkup", AppointmentState.Planned, 101, payment);
             _appointmentDAO.AddApppointment(exampleA);*/
           
            doctor2.Password = BCrypt.Net.BCrypt.HashPassword(doctor2.Password);
            _doctorDAO.AddDoctor(doctor2);
            _scheduleDAO.AddSchedule(doctor2.Schedule, doctor2.Id);
             
            /* MedicalCard card = new MedicalCard();
             Patient p = _patientDAO.GetPatientById(1);
             var a_fake = _appointmentDAO.GetAppointmentByPatientAndTime(p, new DateTime(2024, 05, 11, 15, 58, 16));
             if (a_fake != null)
             {
                 Appointment a = a_fake;
                 Drug paracetamol = new Drug("Paracetamol", "Take one tablet every 4-6 hours as needed for pain", DateTime.Now.AddDays(30));
                 Symptom headache = new Symptom(1, "Headache", "Pain in the head, often in the forehead or temples.");
                 Symptom cough = new Symptom(2, "Cough", "Expelling air from the lungs with a sudden, sharp sound.");
                 cough.Id = 1;
                 headache.Id = 2;
                 paracetamol.Id = 1;

                 Drug ibuprofen = new Drug("Ibuprofen", "Take one or two tablets every 4-6 hours as needed for pain", DateTime.Now.AddDays(60));
                 ibuprofen.Id = 2;
                 Referral referral = new Referral(_doctorDAO.GetDoctorById(2), DateTime.Now.AddDays(30));
                 card.AppointmentRecord.Add(a, (new EMR(referral), new EHR("Patient is ill", new List<Drug> { paracetamol, ibuprofen }, new List<Symptom> { cough, headache })));
                 // _medicalCardDAO.AddMedicalCard(card, p.Id);
                 card.Id = 1;
                 //_symptomDAO.AddSymptom(new List<Symptom> { cough, headache });
                 //_drugDAO.AddDrug(new List<Drug> { paracetamol, ibuprofen });
                 _medicalCardDAO.AddAppointmentRecord(card);
             }*/
            /* doctor2 = _doctorDAO.GetDoctorById(2);
             if (doctor2 != null)
             {
                 doctor2.Schedule = new List<Event> {
                 new Event(DateTime.Today.AddHours(9),DateTime.Today.AddHours(9).AddMinutes(30),null),
                 new Event(DateTime.Today.AddHours(9).AddMinutes(30),DateTime.Today.AddHours(10),null),
                 new Event(DateTime.Today.AddHours(10).AddMinutes(30),DateTime.Today.AddHours(11),null)
                 };
                 _scheduleDAO.AddSchedule(doctor2.Schedule, doctor2.Id);
             }*/
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
