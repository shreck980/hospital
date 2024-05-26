using hospital.Entities;
using hospital.Models;
using hospital.Exceptions;
using hospital.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            PatientService patientService,AppointmentService appointmentService, DoctorService doctorService,
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
            return RedirectToAction("RecordAppointment", "MedicalCard", new { userId, userState});
        }


        [Route("MedicalCard/RecordAppointment/{userId}/{userState}")]
        [Route("MedicalCard/RecordAppointment")]

        [HttpGet]
        public IActionResult RecordAppointment(string userId, string userState)
        {
          
            if (userState is not null && userState == AccountStates.PreActive.ToString())
            {
                return RedirectToAction("BecomeFamilyDoctor", "Doctor", new { id = userId });
            }
            try
            {
                //uint id = uint.Parse(userId);
              
               AppointmentRecordModel model = new AppointmentRecordModel();
               
              
                model.Appointment = _appointmentService.GetAppointmentById(HttpContext.Session.GetInt32("appointmentId").HasValue ? (uint)HttpContext.Session.GetInt32("appointmentId") : 0);
               
                model.Appointment.Patient = _patientService.GetPatientByID(model.Appointment.Patient.Id);
                model.Appointment.Patient.MedicalCard = _medicalCardService.GetMedicalCardEmpty(model.Appointment.Patient.Id);
                model.Appointment.Patient.FamilyDoctor = _doctorService.GetDoctorsByIdMin(model.Appointment.Patient.FamilyDoctor.Id);
                model.Appointment.Doctor = _doctorService.GetDoctorsByIdMin(model.Appointment.Doctor.Id);
                model.EHR.Drugs = _drugService.GetAllDrugs().OrderBy(d=>d.Name).ToList();   
                model.EHR.Symptoms = _symptomService.GetAllSymptoms().OrderBy(s => s.Name).ToList();
                ViewBag.Doctors = _doctorService.GetAllDoctors().Select(d => new
                {
                    Id = d.Id,
                    FullName = d.Name + " " + d.Surname,
                    Speciality = _doctorService.GetSpeciality(d.Speciality)
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
            uint userId = uint.Parse(id);
            Patient p = _patientService.GetPatientByIDSecure(userId);
            return View(p);

        }

        [HttpPost]
        public IActionResult SaveNewMedicalCard(Patient p)
        {
            try
            {
                _medicalCardService.AddMedicalCard(p.Id);
                return RedirectToAction("RecordAppointment", "MedicalCard", new {userState = ((int)p.State).ToString() });
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

            try
            {
                MedicalCard m = new MedicalCard();
                model.EHR.Drugs = model.EHR.Drugs.Where(d => d.IsSelected == true).
                        Select(d =>
                        {
                            d.ExpirationDate = DateTime.Now.AddMonths(3);
                            return d;
                        }).ToList();
                if(model.EMR.Referral.Doctor.Id != 0)
                {
                    model.EMR.Referral.ExpirationDate = DateTime.Now.AddMonths(2);
                }
                model.EHR.Symptoms = model.EHR.Symptoms.Where(s => s.IsSelected == true).ToList();
                model.Appointment.State = AppointmentState.Attended;
                m.AppointmentRecord.Add(model.Appointment, (model.EMR, model.EHR));
                m.Id = model.Appointment.Patient.MedicalCard.Id;

                _medicalCardService.AddAppointmentRecord(m);
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
        public IActionResult OpenAppointmentHistoryForPatient()
        {
            try
            {
                int patientId = HttpContext.Session.GetInt32("patientId") ?? 0;
                MedicalCard c = _medicalCardService.GetMedicalCardEmpty((uint)patientId);  
                List<Appointment> appointments = _appointmentService.GetAllPatientAppointmentList((uint)patientId);
                foreach(Appointment a in appointments)
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

        [HttpGet("[controller]/[action]/{appointmentId:int}")]
        public IActionResult ShowAppointmentDetails(int appointmentId)
        {
            return View();
        }



    }
}
