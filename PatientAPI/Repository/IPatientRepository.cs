using Microsoft.AspNetCore.Mvc;
using PatientAPI.Models;
using PatientAPI.Models.DTO;

namespace PatientAPI.Repository
{
    public interface IPatientRepository
    {
        Task<IEnumerable<PatientDTO>> GetPatients();
        Task<IEnumerable<PatientDetails>> GetAllPatients();
        Task<PatientDTO> GetPatient(int id);
        Task<int> CreatePatient(PatientDTO patientDto);
        Task<PatientDetails> RegisterPatientAsync([FromForm] PatientDetails patient, IFormFile imageFile);
        Task<bool> UpdatePatient(int id, PatientDTO patientDto);
        Task<bool> DeletePatient(int id);
    }
}
