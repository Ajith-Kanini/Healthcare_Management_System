namespace PatientAPI.Models.DTO
{
    public class PatientProfileDTO
    {
        public string? LastName { get; set; }
        public string? PatientPhoto { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? Phone { get; set; }
    }
}
