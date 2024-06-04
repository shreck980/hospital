using hospital.Entities;
using hospital.Models;
using hospital.Exceptions;
using hospital.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Enums.Services;

namespace hospital.Controllers
{
    public class MedicalCardController : Controller
    {
        MedicalCardService _medicalCardService;
        PatientService _patientService;
        AppointmentService _appointmentService;
        DoctorService _doctorService;
        DrugService _drugService;
        SymptomService _symptomService;
        public MedicalCardController(MedicalCardService medicalCardService,
            PatientService patientService, AppointmentService appointmentService, DoctorService doctorService,
            DrugService drugService, SymptomService symptomService)
        {
            _medicalCardService = medicalCardService;
            _patientService = patientService;
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _drugService = drugService;
            _symptomService = symptomService;
        }
        public IActionResult Index()
        {
            return View();
        }


        [Route("MedicalCard/RecordAppointmentPrepare/{userId}/{userState}/{appId:int}")]
        [HttpGet]
        public IActionResult RecordAppointmentPrepare(string userId, string userState, int appId)
        {
            HttpContext.Session.SetInt32("appointmentId", appId);
            return RedirectToAction("RecordAppointment", "MedicalCard", new { userId, userState });
        }


        [Route("MedicalCard/RecordAppointment/{userId}/{userState}")]
        [Route("MedicalCard/RecordAppointment/{userId}")]
        [Route("MedicalCard/RecordAppointment")]
        [HttpGet]
        public IActionResult RecordAppointment(string userId, string userState)
        {
            if (!HttpContext.Session.IsAvailable || HttpContext.Session.GetInt32("doctorId") is null)
            {
                TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                return RedirectToAction("LogOut", "Doctor");
            }
           
                if (userState is not null && userState == AccountStates.PreActive.ToString())
                {
                    return RedirectToAction("BecomeFamilyDoctor", "Doctor", new { id = userId });
                }
            
            try
            {
                //long id = long.Parse(userId);

                AppointmentRecordModel model = new AppointmentRecordModel();


                model.Appointment = _appointmentService.GetAppointmentById(HttpContext.Session.GetInt32("appointmentId").HasValue ? (long)HttpContext.Session.GetInt32("appointmentId") : 0);

                model.Appointment.Patient = _patientService.GetPatientByID(model.Appointment.Patient.Id);
                model.Appointment.Patient.MedicalCard = _medicalCardService.GetMedicalCardEmpty(model.Appointment.Patient.Id);
                model.Appointment.Patient.FamilyDoctor = _doctorService.GetDoctorsByIdMin(model.Appointment.Patient.FamilyDoctor.Id);
                model.Appointment.Doctor = _doctorService.GetDoctorsByIdMin(model.Appointment.Doctor.Id);
                model.EHR.Drugs = _drugService.GetAllDrugs().OrderBy(d => d.Name).ToList();
                model.EHR.Symptoms = _symptomService.GetAllSymptoms().OrderBy(s => s.Name).ToList();
                ViewBag.Doctors = _doctorService.GetAllDoctors().Select(d => new
                {
                    Id = d.Id,
                    FullName = d.Name + " " + d.Surname+" - "+d.Speciality.GetDescription()
                }).ToList();
                ;

                return View(model);

            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }
            catch (NoSuchRecord e)
            {
                return RedirectToAction("CreateMedicalCard", "MedicalCard", new { id = userId });
            }


        }

        [HttpGet("[controller]/[action]/{id}")]
        public IActionResult CreateMedicalCard(string id)
        {
            long userId = long.Parse(id);
            Patient p = _patientService.GetPatientByIDSecure(userId);
            return View(p);

        }

        [HttpPost]
        public IActionResult SaveNewMedicalCard(Patient p)
        {
            try
            {
                _medicalCardService.AddMedicalCard(p.Id);
                return RedirectToAction("RecordAppointment", "MedicalCard", new { userState = ((int)p.State).ToString() });
            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }
        }

        [HttpPost]
        public IActionResult SaveAppointmentRecord(AppointmentRecordModel model)
        {
            if (string.IsNullOrEmpty(model.EHR.ResultOfExamination)&& model.Attended)
            {
                TempData["ErrorMessage"] = "Результат осбтеження не може бути пустим";
                return RedirectToAction("RecordAppointment", "MedicalCard", new 
                {   
                      userId = model.Appointment.Patient.Id,
                     userState = model.Appointment.Patient.State
                });
            }
            if (!model.Attended)
            {
                model.Appointment.State = AppointmentState.NotAttended;
                _appointmentService.UpdateState(model.Appointment);
                return RedirectToAction("PersonalProfile", "Doctor");
            }

            try
            {
                
                _medicalCardService.AddAppointmentRecord(model);
                HttpContext.Session.Remove("appointmentId");
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

            return RedirectToAction("PersonalProfile", "Doctor");
        }

        [HttpGet("[controller]/[action]")]
        public IActionResult AddDrugRecord()
        {

            return RedirectToAction("RecordAppointment", "MedicalCard");
        }

        [HttpGet("[controller]/[action]")]
        [HttpGet("[controller]/[action]/{patientId:long}")]
        public IActionResult OpenAppointmentHistoryForPatient(long? patientId)
        {
            if (!patientId.HasValue)
            {
                if (HttpContext.Session.GetInt32("patientId") is null)
                {
                    TempData["ErrorMessage"] = "Час вашої сесії вичерпався, будь ласка зайдіть знову";
                    return RedirectToAction("LogOut", "PatientAccount");
                }
            }
           
            try
            {
                long id = 0;
                if (patientId.HasValue)
                {
                    id = patientId.Value;
                }
                else
                {
                    id = HttpContext.Session.GetInt32("patientId") ?? 0;
                }

                MedicalCard c = _medicalCardService.GetMedicalCardEmpty(id);
                List<Appointment> appointments = _appointmentService.GetAllPatientAppointmentList(id);
                foreach (Appointment a in appointments)
                {
                    c.AppointmentRecord.Add(a, (null, null));
                }
                return View(c);
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

        [HttpGet("[controller]/[action]/{appointmentId:long}/{medicalCardId:long}/{state:int}")]
        public IActionResult ShowAppointmentDetails(long appointmentId, long medicalCardId,int state)
        {
           
            try
            {
                MedicalCard c = new MedicalCard();
                c.Id = medicalCardId;
                Appointment a = _appointmentService.GetAppointmentByIdAllStates(appointmentId);
                a.Patient = _patientService.GetPatientByID(a.Patient.Id);
                (EMR, EHR) details = _medicalCardService.GetAppointmentDetailsPatient(a.Id);
                c.AppointmentRecord.Add(a, details);
                return View(c);
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


        [HttpGet("[controller]/[action]/{appointmentId:long}/{medicalCardId:long}/{state:int}")]
        public IActionResult ShowAppointmentDetailsForDoctor(long appointmentId, long medicalCardId,int state)
        {
           
            try
            {
                MedicalCard c = new MedicalCard();
                c.Id = medicalCardId;
                Appointment a = _appointmentService.GetAppointmentByIdAllStates(appointmentId);
                a.Patient = _patientService.GetPatientByID(a.Patient.Id);
                (EMR, EHR) details = _medicalCardService.GetAppointmentDetailsPatient(a.Id);
                details.Item2.Symptoms = _symptomService.GetAllSymptomsPerAppointment(details.Item2.Id);
                c.AppointmentRecord.Add(a, details);
                return View(c);
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

        [HttpGet("[controller]/[action]/{referralId:long}/{patientId:long}")]
        public IActionResult DeleteReferral(long referralId, long patientId)
        {
            try
            {
                _medicalCardService.DeleteReferral(referralId);
            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }

            return RedirectToAction("OpenAppointmentHistoryForPatient", "MedicalCard", new
            {
                patientId
            });

        }

        [HttpGet("[controller]/[action]/{referralId:long}/{state:int}/{patientId:long}/{appointment:long}")]
      
        public IActionResult UpdateReferralState(long referralId,int state, long patientId,long appointment)
        {
            try
            {
                _medicalCardService.UpdateReferralState(referralId, state, appointment);
            }
            catch (MySQLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Error");
            }

            return RedirectToAction("OpenAppointmentHistoryForPatient", "MedicalCard", new
            {
                patientId
            });

        }
    }
  }
