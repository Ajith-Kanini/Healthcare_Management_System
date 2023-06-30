using System.ComponentModel.DataAnnotations;

namespace PatientAPI.Models.DTO
{
    public class PatientDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PatientPassword { get; set; }
    }
}
