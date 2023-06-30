using PatientAPI.Models;

namespace PatientAPI.Repository
{
    public interface IAppoinmentBookingRepository
    {
        Task<IEnumerable<AppoinmentBooking>> GetAppoinmentBookingsAsync();
        Task<AppoinmentBooking> GetAppoinmentBookingByIdAsync(int id);
        Task CreateAppoinmentBookingAsync(AppoinmentBooking appoinmentBooking);
        Task UpdateAppoinmentBookingAsync(AppoinmentBooking appoinmentBooking);
        Task DeleteAppoinmentBookingAsync(AppoinmentBooking appoinmentBooking);
        bool AppoinmentBookingExists(int id);
    }
}
