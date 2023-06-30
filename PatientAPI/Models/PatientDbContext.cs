using Microsoft.EntityFrameworkCore;

namespace PatientAPI.Models
{
    public class PatientDbContext:DbContext
    {
         public DbSet<PatientDetails> patientDetails { get; set; }
        public DbSet<AppoinmentBooking> appoinmentBookings { get; set; }

        public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
        {

        }
    }
}
