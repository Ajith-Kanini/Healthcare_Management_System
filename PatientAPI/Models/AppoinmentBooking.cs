using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PatientAPI.Models
{
    public class AppoinmentBooking
    {
        [Key]
        public int AppointmentId { get; set; }
        public string? Diagnose { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
