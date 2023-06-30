
using Microsoft.EntityFrameworkCore;
using PatientAPI.Models;
using PatientAPI.Repository;

public class AppoinmentBookingRepository : IAppoinmentBookingRepository
{
    private readonly PatientDbContext _context;

    public AppoinmentBookingRepository(PatientDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AppoinmentBooking>> GetAppoinmentBookingsAsync()
    {
        return await _context.appoinmentBookings.ToListAsync();
    }

    public async Task<AppoinmentBooking> GetAppoinmentBookingByIdAsync(int id)
    {
        return await _context.appoinmentBookings.FindAsync(id);
    }

    public async Task CreateAppoinmentBookingAsync(AppoinmentBooking appoinmentBooking)
    {
        _context.appoinmentBookings.Add(appoinmentBooking);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAppoinmentBookingAsync(AppoinmentBooking appoinmentBooking)
    {
        _context.Entry(appoinmentBooking).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAppoinmentBookingAsync(AppoinmentBooking appoinmentBooking)
    {
        _context.appoinmentBookings.Remove(appoinmentBooking);
        await _context.SaveChangesAsync();
    }

    public bool AppoinmentBookingExists(int id)
    {
        return _context.appoinmentBookings.Any(e => e.AppointmentId == id);
    }
}