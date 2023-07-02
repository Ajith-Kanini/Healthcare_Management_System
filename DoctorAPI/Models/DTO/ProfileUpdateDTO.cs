namespace DoctorAPI.Models.DTO
{
    public class ProfileUpdateDTO
    {
        
        public string? DoctorImage { get; set; }
        public string? Specialization { get; set; }
        public int? ExperienceYears { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? Phone { get; set; }
        public bool? Availability { get; set; }
    }
}
