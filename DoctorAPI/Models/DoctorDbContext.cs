using Microsoft.EntityFrameworkCore;

namespace DoctorAPI.Models
{
    public class DoctorDbContext:DbContext
    {
        public DbSet<DoctorDetails> doctorDetails { get; set; }

        public DoctorDbContext(DbContextOptions<DoctorDbContext> options) : base(options)
        {

        }
    }

}
