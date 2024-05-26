using hospital.DAO;
using hospital.Entities;
using hospital.Exceptions;
using hospital.Models;
using hospital.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Numerics;
using System.Reflection;

namespace hospital.Controllers
{
    public class DoctorController : Controller
    {
        DoctorService _doctorService;
        AppointmentService _appointmentService;
        PatientService _patientService;
        ScheduleService _scheduleService;

        public DoctorController(DoctorService doctorService, ScheduleService scheduleService, AppointmentService appointmentService, PatientService patientService)
        {
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _appointmentService = appointmentService;
            _patientService = patientService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ChooseFamilyDoctor()
        {
            if (HttpContext.Session.GetInt32("patientId") is null)
            {
                TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                return RedirectToAction("Login", "PatientAccount");
            }


            /*if (HttpContext.Session.GetInt32("isLogged") == 0)
            {
                TempData["ErrorMessage"] = "Для того щоб мати доступ до цієї функції треба бути авторизованим. Будь ласка авторизуйтесь";
                return RedirectToAction("Error", "Error");
            }*/

            try
            {
                List<Doctor> doctors = _doctorService.GetDoctorsBySpeciality(Speciality.Therapist);

                ViewBag.Doctors = doctors.Select(d => new
                {
                    Id = d.Id,
                    FullName = d.Name + " " + d.Surname
                }).ToList();
                ;


            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }

            return View(new BookAppointment());
        }
        public IActionResult ChooseDoctor()
        {
            if (HttpContext.Session.GetInt32("patientId") is null)
            {
                TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                return RedirectToAction("Login", "PatientAccount");
            }


            /*if (HttpContext.Session.GetInt32("isLogged") == 0)
            {
                TempData["ErrorMessage"] = "Для того щоб мати доступ до цієї функції треба бути авторизованим. Будь ласка авторизуйтесь";
                return RedirectToAction("Error", "Error");
            }*/

            try
            {
                List<Doctor> doctors = _doctorService.GetAllDoctorsForPatient();
                int patientId = HttpContext.Session.GetInt32("patientId") ?? 0;
               
                uint family_doctor_id = _patientService
                    .GetPatientByID((uint)patientId)
                    .FamilyDoctor.Id;
                doctors.Add(_doctorService.GetDoctorsById(family_doctor_id));

                ViewBag.Doctors = doctors.Select(d => new
                {
                    Id = d.Id,
                    FullName = d.Name + " " + d.Surname+" - "+ _doctorService.GetSpeciality(d.Speciality)
                }).ToList();
                ;


            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }

            return View(new BookAppointment());
        }

        [HttpPost]
        public IActionResult BookFirstAppointmentSchedule(BookAppointment model, int? doctorId)
        {
            if (HttpContext.Session.GetInt32("patientId") is null)
            {
                TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                return RedirectToAction("Login", "PatientAccount");
            }

            if (model.Doctor.Id == 0)
            {

                TempData["ErrorMessage"] = "Будь ласка оберіть лікаря";
                TempData["SpecialLink"] = Url.Action("ChooseFamilyDoctor", "Doctor");
                return RedirectToAction("Error", "Error");
            }

            try
            {
                if (doctorId.HasValue)
                {
                    model.Doctor.Id = (uint)doctorId.Value;
                }

                if (!_scheduleService.RetrieveScheduleForPatientView(model))
                {
                    model.Doctor.Id = 0;
                    TempData["ErrorMessage"] = "Лікар не має вільних годин роботи,будь ласка оберіть іншого лікаря";
                    return RedirectToAction("Error", "Error");
                }


                return View(model);


            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }
            catch (NoSuchRecord e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }

        }

        [HttpGet("[controller]/[action]/{doctorId:int}")]
        public IActionResult BookAppointmentSchedule(int doctorId)
        {
            if (HttpContext.Session.GetInt32("patientId") is null)
            {
                TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                return RedirectToAction("Login", "PatientAccount");
            }

           

            try
            {
                BookAppointment model = new BookAppointment();
                model.Doctor.Id = (uint)doctorId;

                if (!_scheduleService.RetrieveScheduleForPatientView(model))
                {
                    model.Doctor.Id = 0;
                    TempData["ErrorMessage"] = "Лікар не має вільних годин роботи,будь ласка оберіть іншого лікаря";
                    return RedirectToAction("Error", "Error");
                }


                return View(model);


            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }
            catch (NoSuchRecord e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }

        }

        [HttpPost]
        public IActionResult BookAppointmentSchedule(BookAppointment model)
        {
            if (HttpContext.Session.GetInt32("patientId") is null)
            {
                TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                return RedirectToAction("Login", "PatientAccount");
            }

            if (model.Doctor.Id == 0)
            {

                TempData["ErrorMessage"] = "Будь ласка оберіть лікаря";
                TempData["SpecialLink"] = Url.Action("ChooseDoctor", "Doctor");
                return RedirectToAction("Error", "Error");
            }

            try
            {


                if (!_scheduleService.RetrieveScheduleForPatientView(model))
                {
                    model.Doctor.Id = 0;
                    TempData["ErrorMessage"] = "Лікар не має годин роботи,будь ласка оберіть іншого лікаря";
                    return RedirectToAction("Error", "Error");
                }


                return View(model);


            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }
            catch (NoSuchRecord e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }

        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("doctorId");
            return RedirectToAction("StartPage", "Start");
        }

        public IActionResult CheckAccount(Account model)
        {
            try
            {
                Doctor doctor = _doctorService.GetDoctorsByEmail(model.Email);
                if (doctor.State == AccountStates.Frozen || doctor.State == AccountStates.Unverified || doctor.State == AccountStates.PreRemoved)
                {
                    ModelState.AddModelError("ErrorMessage", "Ваш акаунт є недійсним, будь ласка зверніться до адміністрації");
                    return View("~/Views/Doctor/Login.cshtml", model);
                }
                if (BCrypt.Net.BCrypt.Verify(model.Password, doctor.Password))
                {
                    HttpContext.Session.SetInt32("doctorId", (int)doctor.Id);

                    return RedirectToAction("PersonalProfile", "Doctor");
                }


                ModelState.AddModelError("ErrorMessage", "Невірний пароль");
                return View("~/Views/Doctor/Login.cshtml", model);
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
            catch (NoSuchRecord e)
            {
                ModelState.AddModelError("ErrorMessage", e.Message);
                return View("~/Views/Doctor/Login.cshtml", model);
                //throw new NoSuchRecord(e.Message, e);
            }

        }
    
        public IActionResult PersonalProfile()
        {

            if (HttpContext.Session.GetInt32("doctorId") is null)
            {
                TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                return RedirectToAction("Login", "Doctor");
            }
            uint id = HttpContext.Session.GetInt32("doctorId").HasValue ? (uint)HttpContext.Session.GetInt32("doctorId") : 0;
            try
            {
                Doctor doctor = _doctorService.GetDoctorsById(id);
                doctor.Schedule = _scheduleService.RetrieveScheduleForDoctorView(id);
                if (doctor.Schedule.Count == 0)
                {
                    ModelState.AddModelError("no_schedule", "У вас поки нема записаних прийомів");
                    return View(doctor);
                }
                foreach (Event e in doctor.Schedule)
                {
                    e.Appointment = _appointmentService.GetAppointmentById(e.Appointment.Id);
                    e.Appointment.Patient = _patientService.GetPatientMinById(e.Appointment.Patient.Id);

                }
                return View(doctor);
            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }
            catch (NoSuchRecord e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }
        }

        // [Route("Doctor/BecomeFamilyDoctor/{id}")]
        //[HttpGet]
        [HttpGet("[controller]/[action]/{id}")]
        public IActionResult BecomeFamilyDoctor(string id)
        {
            uint userId = uint.Parse(id);
            Patient p = _patientService.GetPatientMinById(userId);
            return View(p);
            
        }

        [HttpPost]
        public IActionResult SaveFamilyDoctorChoice(IFormCollection form, Patient p)
        {
            if (HttpContext.Session.GetInt32("doctorId") is null)
            {
                TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                return RedirectToAction("Login", "Doctor");
            }
            try
            {
                if (form["familyDoctor"] == "on")
                {

                    p.FamilyDoctor.Id = HttpContext.Session.GetInt32("doctorId").HasValue ? (uint)HttpContext.Session.GetInt32("doctorId") : 0;
                    p.State = AccountStates.Active;
                    _patientService.SetFamilyDoctor(p);
                }
                else
                {
                    TempData["ErrorMessage"] = "Ви не можете створити медицинську картку пацієнта, якщо ви не є його сімейним лікарем";
                    return RedirectToAction("BecomeFamilyDoctor", "Doctor", new { id = p.Id.ToString() });
                }
                
                return RedirectToAction("RecordAppointment", "MedicalCard", new { userId = p.Id, userState = ((int)p.State).ToString() });
            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }
            
        }



    }
}
