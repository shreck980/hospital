using hospital.DAO;
using hospital.Entities;
using hospital.Models;
using hospital.Exceptions;
using System.Reflection;

namespace hospital.Services
{
    public class MedicalCardService
    {
        IMedicalCardDAO _medicalCardDAO;
        IAppointmentDAO _appointmentDAO;
        public MedicalCardService(IMedicalCardDAO medicalCardDAO, IAppointmentDAO appointmentDAO )
        {
            _medicalCardDAO = medicalCardDAO;
            _appointmentDAO = appointmentDAO;
        }
        public MedicalCard GetMedicalCardEmpty(long id)
        {
            try
            {
                MedicalCard a = _medicalCardDAO.GetMedicalCardEmpty(id);
                return a;

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
            catch (NoSuchRecord e)
            {
                throw new NoSuchRecord(e.Message, e);
            }
        }
        public MedicalCard GetMedicalCardForPatient(long id)
        {
            try
            {
                MedicalCard a = _medicalCardDAO.GetMedicalCardPatient(id);
                return a;

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
            catch (NoSuchRecord e)
            {
                throw new NoSuchRecord(e.Message, e);
            }
        }
        public void AddMedicalCard(long patientId)
        {
            try
            {
                _medicalCardDAO.AddMedicalCard(patientId);

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
           
        }
        public void AddAppointmentRecord(AppointmentRecordModel model)
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
               
                if (model.EMR.Referral.Doctor.Id != 0)
                {
                    model.EMR.Referral.ExpirationDate = DateTime.Now.AddMonths(2);
                    //model.EMR.Referral.AppointmetnId = model.Appointment.Id;
                }
                if (model.HasPayment)
                {
                    model.Appointment.Payment = new Payment();
                    model.Appointment.Payment.Price = 0;
                    model.Appointment.Payment.Patient.Id = model.Appointment.Patient.Id;
                    model.Appointment.Payment.DateIssued = DateTime.Now;
                    _appointmentDAO.PaymentToApppointment(model.Appointment.Payment, model.Appointment.Id);
                }
                model.EHR.Symptoms = model.EHR.Symptoms.Where(s => s.IsSelected == true).ToList();
                if (model.Appointment.State == AppointmentState.PlannedByReferral)
                {
                    _medicalCardDAO.UpdateReferralState(_medicalCardDAO.GetReferralIdByAppointmentId(m.AppointmentRecord.Keys.First().Id), (int)ReferralState.Visited, null);
                }
                model.Appointment.State = AppointmentState.Attended;
                m.AppointmentRecord.Add(model.Appointment, (model.EMR, model.EHR));
                m.Id = model.Appointment.Patient.MedicalCard.Id;

                _medicalCardDAO.AddAppointmentRecord(m);
               

            }
            catch (NoSuchRecord e)
            {
                throw new NoSuchRecord(e.Message, e);
            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }

        }

        public (EMR, EHR) GetAppointmentDetailsPatient(long appointment)
        {
            try
            {
                return _medicalCardDAO.GetAppointmentDetailsPatient(appointment);

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
            catch (NoSuchRecord e)
            {
                throw new NoSuchRecord(e.Message, e);
            }
        }

        public void DeleteReferral(long id)
        {
            try
            {
                _medicalCardDAO.DeleteReferral(id);

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
        }

        public void UpdateReferralState(long referralId, int state, long? appointment)
        {
            try
            {
                _medicalCardDAO.UpdateReferralState( referralId,  state, appointment);

            }
            catch (MySQLException e)
            {
                throw new MySQLException(e.Message, e);
            }
        }


    }
}
