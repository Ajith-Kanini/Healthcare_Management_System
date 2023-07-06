using PatientAPI.Models.DTO;
using PatientAPI.Models;
using PatientAPI.Repository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

public class PatientRepository : IPatientRepository
{
    private readonly PatientDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PatientRepository(PatientDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IEnumerable<PatientDTO>> GetPatients()
    {
        var patients = await _context.patientDetails
            .Select(patient => new PatientDTO
            {
                FirstName = patient.FirstName,
                Email = patient.Email,
            })
            .ToListAsync();

        return patients;
    }
    public async Task<IEnumerable<PatientDetails>> GetAllPatients()
    {
        var patients = await _context.patientDetails.ToListAsync();

        return patients;
    }

    public async Task<PatientDetails> GetPatient(int id)
    {
        var patient = await _context.patientDetails.FirstOrDefaultAsync();

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

    public async Task<PatientDetails> RegisterPatientAsync([FromForm] PatientDetails patient, IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            throw new ArgumentException("Invalid file");
        }

        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            patient.PatientPhoto = fileName;

            _context.patientDetails.Add(patient);
            _context.SaveChanges();

            return patient;
        }
        catch (Exception ex)
        {
            // Rethrow the exception with additional information
            throw new Exception("Error occurred while posting the room.", ex);
        }
    }
    public async Task<PatientDetails> PutPatientProfile(int id, [FromForm] PatientProfileDTO dto, IFormFile imageFile)
    {
        var existingDoctor = await _context.patientDetails.FindAsync(id);

        if (existingDoctor == null)
        {
            throw new ArgumentException("Doctor not found");
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            if (!string.IsNullOrEmpty(existingDoctor.PatientPhoto))
            {
                var existingFilePath = Path.Combine(uploadsFolder, existingDoctor.PatientPhoto);
                if (File.Exists(existingFilePath))
                {
                    File.Delete(existingFilePath);
                }
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            dto.PatientPhoto = fileName;
        }
        else
        {
            dto.PatientPhoto = existingDoctor.PatientPhoto;
        }

        existingDoctor.LastName = dto.LastName;
        existingDoctor.Age = dto.Age;
        existingDoctor.Phone = dto.Phone;
        existingDoctor.Gender = dto.Gender;
        existingDoctor.State = dto.State;
        existingDoctor.Address = dto.Address;
        await _context.SaveChangesAsync();

        return existingDoctor;

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
