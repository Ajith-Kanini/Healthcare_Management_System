using DoctorAPI.Models;

namespace DoctorAPI.Repository
{
    public interface IDoctorDetailsRepository
    {
        Task<IEnumerable<DoctorDetails>> GetAllDoctorDetailsAsync();
        Task<DoctorDetails> GetDoctorDetailsByIdAsync(int id);
        Task CreateDoctorDetailsAsync(DoctorDetails doctorDetails);
        Task UpdateDoctorDetailsAsync(DoctorDetails doctorDetails);
        Task DeleteDoctorDetailsAsync(int id);
    }
}
