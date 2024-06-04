using hospital.Entities;
using hospital.Exceptions;
using hospital.Models;
using hospital.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace hospital.Controllers
{
    public class AppointmentController : Controller
    {
        AppointmentService _appointmentService;
        ScheduleService _scheduleService;
        PatientService _patientService;
        public AppointmentController(AppointmentService appointmentService, ScheduleService scheduleService, PatientService patientService)
        {
            _appointmentService = appointmentService;
            _scheduleService = scheduleService;
            _patientService = patientService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SaveFirstAppointment(BookAppointment model)
        {
            if (HttpContext.Session.GetInt32("patientId") is null)
            {
                TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                return RedirectToAction("LogOut", "PatientAccount");
            }


          

            if (!ModelState.IsValid)
            {

               

                try
                {
                    if (!_scheduleService.RetrieveScheduleForPatientView(model))
                    {
                        model.Doctor.Id = 0;
                        TempData["ErrorMessage"] = "Лікар не має годин роботи,будь ласка оберіть іншого лікаря";
                        return RedirectToAction("Error", "Error");
                    }

                    return View("~/Views/Doctor/BookFirstAppointmentSchedule.cshtml", model);

                }
                catch (MySQLException e)
                {
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Error");
                }
            }

            long? id = (long)HttpContext.Session.GetInt32("patientId");
            if(id == null)
            {
                TempData["ErrorMessage"] ="Ваша сесія вичерпалась, будь ласка ввійдіть ше раз";
                return RedirectToAction("Error", "Error");
            }
            _appointmentService.AddAppointment(model,id);
            _patientService.UpdateState(AccountStates.PreActive,(long)id);

            return RedirectToAction("PersonalProfile", "PatientAccount");
        }

        [HttpPost]
        public IActionResult SaveAppointment(BookAppointment model)
        {
            if (HttpContext.Session.GetInt32("patientId") is null)
            {
                TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                return RedirectToAction("LogOut", "PatientAccount");
            }

            if (!ModelState.IsValid)
            {

                try
                {
                    if (!_scheduleService.RetrieveScheduleForPatientView(model))
                    {
                        model.Doctor.Id = 0;
                        TempData["ErrorMessage"] = "Лікар не має годин роботи,будь ласка оберіть іншого лікаря";
                        return RedirectToAction("Error", "Error");
                    }
                   

                    return View("~/Views/Doctor/BookFirstAppointmentSchedule.cshtml", model);

                }
                catch (MySQLException e)
                {
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Error");
                }
            }

            long? id = (long)HttpContext.Session.GetInt32("patientId");
            if (id == null)
            {
                TempData["ErrorMessage"] = "Ваша сесія вичерпалась, будь ласка ввійдіть ше раз";
                return RedirectToAction("Error", "Error");
            }
            
               long appointmentId= _appointmentService.AddAppointment(model, id);

            if (model.ReferralId.HasValue)
            {
                return RedirectToAction("UpdateReferralState", "MedicalCard", new
                {
                    referralId = model.ReferralId.Value,
                    state = (int)ReferralState.Scheduled,
                    patientId = HttpContext.Session.GetInt32("patientId"),
                    appointment = appointmentId
                }); ;

            }

            return RedirectToAction("PersonalProfile", "PatientAccount");
        }

        [HttpGet("[controller]/[action]/{appointmentId:int}/{patientId:int}/{state:int}")]
        public IActionResult CancelAppointment(int appointmentId, int patientId,int state)
        {
            if (state == 3 || state == 4)
            {
                TempData["ErrorMessage"] = "Сталася помилка, спробуйте пізніше";
                return RedirectToAction("Error", "Error");
            }
            try
            {
                _appointmentService.CancelAppointment((long)appointmentId);
            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }
            return RedirectToAction("OpenAppointmentHistoryForPatient", "MedicalCard", new { 
            patientId
            });

        }


    }
}
