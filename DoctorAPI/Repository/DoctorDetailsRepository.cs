using DoctorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorAPI.Repository
{
    public class DoctorDetailsRepository : IDoctorDetailsRepository
    {
        private readonly DoctorDbContext _context;

        public DoctorDetailsRepository(DoctorDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorDetails>> GetAllDoctorDetailsAsync()
        {
            return await _context.doctorDetails.ToListAsync();
        }

        public async Task<DoctorDetails> GetDoctorDetailsByIdAsync(int id)
        {
            return await _context.doctorDetails.FindAsync(id);
        }

        public async Task CreateDoctorDetailsAsync(DoctorDetails doctorDetails)
        {
            _context.doctorDetails.Add(doctorDetails);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDoctorDetailsAsync(DoctorDetails doctorDetails)
        {
            _context.Entry(doctorDetails).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDoctorDetailsAsync(int id)
        {
            var doctorDetails = await _context.doctorDetails.FindAsync(id);
            if (doctorDetails != null)
            {
                _context.doctorDetails.Remove(doctorDetails);
                await _context.SaveChangesAsync();
            }
        }
    }
}
