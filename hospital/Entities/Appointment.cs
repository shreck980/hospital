namespace hospital.Entities
{
    public class Appointment
    {
        public uint Id { get; set; }
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime TimeStart { get; set; }
        public string ReasonForAppeal { get; set; }
        public AppointmentState State { get; set; }
        public uint RoomNumber {  get; set; }
        public Payment Payment { get; set; }    
        public Appointment()
        {

        }
        public Appointment(uint id, Patient patient, Doctor doctor, DateTime timeStart, string reasonForAppeal, AppointmentState state, uint roomNumber, Payment payment)
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

        public void UpdateState(AppointmentState newState)
        {
            State = newState;
        }

    }
}

public enum AppointmentState
{
    Reserved,
    Planned,
    Attended,
    NotAttended,

}
