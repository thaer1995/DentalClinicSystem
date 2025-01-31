namespace DentalClinicSystem.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MedicalHistory { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
