using PatientAPI.Models.DTO;
using PatientAPI.Models;
using PatientAPI.Repository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

public class PatientRepository : IPatientRepository
{
    private readonly PatientDbContext _context;

    public PatientRepository(PatientDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PatientDTO>> GetPatients()
    {
        var patients = await _context.patientDetails
            .Select(patient => new PatientDTO
            {
                FirstName = patient.FirstName,
                Email = patient.Email,
                PatientPassword = patient.PatientPassword
            })
            .ToListAsync();

        return patients;
    }

    public async Task<PatientDTO> GetPatient(int id)
    {
        var patient = await _context.patientDetails
            .Where(p => p.PatientId == id)
            .Select(patient => new PatientDTO
            {
                FirstName = patient.FirstName,
                Email = patient.Email,
                PatientPassword = patient.PatientPassword
            })
            .FirstOrDefaultAsync();

        return patient;
    }

    public async Task<int> CreatePatient(PatientDTO patientDto)
    {
        var patient = new PatientDetails
        {
            FirstName = patientDto.FirstName,
            Email = patientDto.Email,
            PatientPassword = Encrypt(patientDto.PatientPassword)
        };

        _context.patientDetails.Add(patient);
        await _context.SaveChangesAsync();

        return patient.PatientId;
    }

    public async Task<bool> UpdatePatient(int id, PatientDTO patientDto)
    {
        var patient = await _context.patientDetails.FindAsync(id);

        if (patient == null)
            return false;

        patient.FirstName = patientDto.FirstName;
        patient.Email = patientDto.Email;
        patient.PatientPassword = patientDto.PatientPassword;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeletePatient(int id)
    {
        var patient = await _context.patientDetails.FindAsync(id);

        if (patient == null)
            return false;

        _context.patientDetails.Remove(patient);
        await _context.SaveChangesAsync();

        return true;
    }
    private string Encrypt(string password)
    {
        // Example key and IV generation using hashing
        string passphrase = "your-passphrase";

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(passphrase));
            byte[] iv = sha256.ComputeHash(Encoding.UTF8.GetBytes(passphrase)).Take(16).ToArray();

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(password);
                        }
                    }

                    byte[] encryptedData = memoryStream.ToArray();
                    return Convert.ToBase64String(encryptedData);
                }
            }
        }
    }
}
