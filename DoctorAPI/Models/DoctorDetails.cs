using System.ComponentModel.DataAnnotations;

namespace DoctorAPI.Models
{
    public class DoctorDetails
    {
        [Key]
        public int DoctorId { get; set; }
        public string DoctoName { get; set; }=string.Empty;
        public string? DoctorImage { get; set; }
        public string? Specialization { get; set; }
        public int? ExperienceYears { get; set; } 
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public  bool? RequestStatus { get; set; }
        public string? Availability { get; set; }
        public string? DoctorPassword { get; set; }
    }
}
