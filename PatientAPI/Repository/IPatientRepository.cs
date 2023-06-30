using PatientAPI.Models.DTO;

namespace PatientAPI.Repository
{
    public interface IPatientRepository
    {
        Task<IEnumerable<PatientDTO>> GetPatients();
        Task<PatientDTO> GetPatient(int id);
        Task<int> CreatePatient(PatientDTO patientDto);
        Task<bool> UpdatePatient(int id, PatientDTO patientDto);
        Task<bool> DeletePatient(int id);
    }
}
