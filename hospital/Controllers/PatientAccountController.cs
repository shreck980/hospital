using Microsoft.AspNetCore.Mvc;
using hospital.Entities;
using hospital.Models;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Xml;
using hospital.Services;
using hospital.DAO;
using hospital.Exceptions;
using System.Numerics;
using Microsoft.AspNetCore.SignalR;

namespace hospital.Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]



    public class PatientAccountController : Controller
    {
        PatientService _patientService;
        AppointmentService _appointmentService;
        
        public PatientAccountController(PatientService patientService,AppointmentService appointmentService) {
            _patientService= patientService;
            _appointmentService= appointmentService;
           
        }

        public IActionResult SignUp()
        {
            Patient p = new Patient();
            return View(p);
        }
        public IActionResult Login()
        {
            Account a = new Account();
            return View(a);
        }

        [HttpPost]
        public IActionResult CheckAccount(Account model)
        {
            if (ModelState.IsValid) {
                try
                {
                    Patient p1 = _patientService.GetPatientByEmail(model.Email);

                    if (BCrypt.Net.BCrypt.Verify(model.Password, p1.Password))
                    {

                        
                        HttpContext.Session.SetInt32("patientId", (int)p1.Id);
                        TempData["State"] = p1.State;
                        return RedirectToAction("PersonalProfile", "PatientAccount");
                    }
                    else
                    {
                        ModelState.AddModelError("incorect_login_data", "Невірний пароль чи пошта. Будь ласка спробуйте ще раз");
                        return View("~/Views/PatientAccount/Login.cshtml", model);
                    }

                }
                catch (MySQLException e)
                {
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Error");
                }
                catch (NoSuchRecord e)
                {
                    ModelState.AddModelError("incorect_login_data", e.Message);
                    return View("~/Views/PatientAccount/Login.cshtml", model);
                }
            }
            return View("~/Views/PatientAccount/Login.cshtml", model);

        }

        public IActionResult AccountVerification(string email)
        {
          Console.WriteLine(email);

            try
            {
                _patientService.VerificateAccount(email);
            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }

            return View("~/Views/PatientAccount/AccountVerification.cshtml");
        }

        public IActionResult EmailSent()
        {
            return View();
        }


        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            //HttpContext.Session.Remove("patientId");
            return RedirectToAction("StartPage", "Start");
        }



        public IActionResult PersonalProfile()
        {
            //int? isLogged = 1;

            if (HttpContext.Session.GetInt32("patientId") is null)
            {
                TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                return RedirectToAction("LogOut", "PatientAccount");
            }

           
            

            long id = (long)HttpContext.Session.GetInt32("patientId");
            //long id =3;
            try
            {
                Patient p = _patientService.GetPatientByID(id);
                TempData["State"] = p.State;
                List<Appointment> appointments = _appointmentService.GetPatientAppointmentList(id);
                var model = new
                {
                    Name = p.Name,
                    Surname = p.Surname,
                    FamilyDoctor = p.FamilyDoctor.Id,
                    Appointments = appointments
                };
                if (appointments.Count == 0)
                {
                    ModelState.AddModelError("no_schedule", " У вас нема запланованих прийомів");
                    return View(model);
                }
                

                return View(model);
            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }
           


        }


        [HttpPost]
        public IActionResult CreateUnverified(Patient model)
        {

            if (!ModelState.IsValid)
            {
                // Return to the same view with validation errors
                return View("~/Views/PatientAccount/SignUp.cshtml", model);
            }

            try
            {
               _patientService.CreateUnverifiedAccount(model);
            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }
            catch (SmtpException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }


                return RedirectToAction("EmailSent", "PatientAccount"); // Redirect to home page after successful submission
            
            
        }


    }
}
