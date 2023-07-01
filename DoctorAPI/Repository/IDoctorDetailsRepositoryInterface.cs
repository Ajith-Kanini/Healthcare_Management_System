using DoctorAPI.Models;
using DoctorAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAPI.Repository
{
    public interface IDoctorDetailsRepository
    {
        Task<IEnumerable<DoctorDetails>> GetAllDoctorDetailsAsync();
        Task<DoctorDetails> GetDoctorDetailsByIdAsync(int id);
        Task CreateDoctorDetailsAsync(DoctorDetails doctorDetails);
        Task<DoctorDetails> RegisterDoctorAsync([FromForm] DoctorDetails doctor, IFormFile imageFile);
        Task UpdateDoctorDetailsAsync(DoctorDetails doctorDetails);
        Task DeleteDoctorDetailsAsync(int id);
        Task<DoctorDetails> PutDoctorDetails(int id, UpdatestatusDTO dto);
    }
}
