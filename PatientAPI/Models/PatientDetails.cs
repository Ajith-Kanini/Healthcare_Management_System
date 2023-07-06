using System.ComponentModel.DataAnnotations;

namespace PatientAPI.Models
{
    public class PatientDetails
    {
        [Key]
        public int PatientId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? PatientPhoto { get; set; }
        public int? Age { get; set; } = 0;
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PatientPassword { get; set; }
    }
}
