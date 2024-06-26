﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace hospital.Entities
{
    public class Appointment
    {
        public long Id { get; set; }
        [ValidateNever]
        public Patient Patient { get; set; }
        // [ValidateNever]
        [ValidateNever]
        [Display(Name = "Лікар")]
        public Doctor Doctor { get; set; }
        [Required(ErrorMessage = "Ви повинні обрати години прийому")]
        [Display(Name = "Час")]
        public DateTime TimeStart { get; set; }
        [Required(ErrorMessage = "Reason for appointment is required")]
        [Display(Name = "Причина звернення")]
        public string ReasonForAppeal { get; set; }
        [ValidateNever]
        [Display(Name = "Статус")]
        public AppointmentState State { get; set; }
        [ValidateNever]
        [Display(Name = "Кабінет")]
        public long RoomNumber {  get; set; }
        [ValidateNever]
        public Payment? Payment { get; set; }    
        public Appointment()
        {
            Patient  =new Patient();
            Doctor = new Doctor();
            Payment=new Payment();
        }
        public Appointment(long id, Patient patient, Doctor doctor, DateTime timeStart, string reasonForAppeal, AppointmentState state, long roomNumber, Payment payment)
        {
            Id = id;
            Patient = patient;
            Doctor = doctor;
            TimeStart = timeStart;
            ReasonForAppeal = reasonForAppeal;
            State = state;
            RoomNumber = roomNumber;
            Payment = payment;
        }

        public Appointment(Patient patient, Doctor doctor, DateTime timeStart, string reasonForAppeal, AppointmentState state, long roomNumber, Payment payment)
        {
            Id = 0;
            Patient = patient;
            Doctor = doctor;
            TimeStart = timeStart;
            ReasonForAppeal = reasonForAppeal;
            State = state;
            RoomNumber = roomNumber;
            Payment = payment;
        }

        public void UpdateState(AppointmentState newState)
        {
            State = newState;
        }

        public override string ToString()
        {
            return $"Id: {Id}\n" +
                   $"Patient: {Patient}\n" +
                   $"Doctor: {Doctor}\n" +
                   $"TimeStart: {TimeStart}\n" +
                   $"ReasonForAppeal: {ReasonForAppeal}\n" +
                   $"State: {State}\n" +
                   $"RoomNumber: {RoomNumber}\n" +
                   $"Payment: {Payment}";
        }

      

    }
}

public enum AppointmentState
{
    [Description("Зарезервовано")]
    Reserved=1,
    [Description("Заплановано")]
    Planned,
    [Description("Відвідано")]
    Attended,
    [Description("Не відвідано")]
    NotAttended,
    [Description("По направленню")]
    PlannedByReferral

}
