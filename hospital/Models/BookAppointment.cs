using hospital.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.RegularExpressions;

namespace hospital.Models
{
    public class BookAppointment
    {

       
        public Doctor Doctor { get; set; }

        [Required(ErrorMessage = "Ви повинні обрати години прийому")]
        [Range(0, long.MaxValue, ErrorMessage = "Ви повинні обрати години прийому")]
        public long? ScheduleId {  get; set; }
       
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(80, ErrorMessage = "Причина запису на прийом не може бути довше 80 символів")]
        public string? ReasonForAppeal { get; set; }
        public List<SelectListItem> Schedule {  get; set; }
        public long? ReferralId { get; set; }
        public void ScheduleForView()
        {
           var sortedSchedule = Doctor.Schedule.OrderBy(s => s.Start.Hour).ToList();
           var groups = new Dictionary<string, SelectListGroup>();

            // Iterate over your schedule entries and add them to the SelectList
            foreach (var e in sortedSchedule)
            {
                // Convert the date to a string to use as a key
                string dateKey = e.Start.ToShortDateString();

                // Check if a group for this date already exists, if not create one
                if (!groups.ContainsKey(dateKey))
                {
                    groups[dateKey] = new SelectListGroup { Name = dateKey };
                }

                // Add the SelectListItem with the appropriate group
                Schedule.Add(new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.Start.ToShortTimeString()} - {e.End.ToShortTimeString()}",
                    Group = groups[dateKey]
                });


            }
        }

        public BookAppointment()
        {
            ReasonForAppeal = "";
            ScheduleId = 0;
            Doctor=new Doctor();
            Schedule = new List<SelectListItem>();
        }


    }
}