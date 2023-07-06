using DoctorAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Humanizer;
using DoctorAPI.Models.DTO;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;

namespace DoctorAPI.Repository
{
    public class DoctorDetailsRepository : IDoctorDetailsRepository
    {
        private readonly DoctorDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DoctorDetailsRepository(DoctorDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<DoctorDetails>> GetAllDoctorDetailsAsync()
        {
            return await _context.doctorDetails.ToListAsync();
        }

        public async Task<DoctorDetails> GetDoctorDetailsByIdAsync(int id)
        {
            return await _context.doctorDetails.FindAsync(id);
        }

        public async Task<DoctorDetails> GetFullDoctorDetailsByIdAsync(int id)
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
        public async Task<DoctorDetails> PutDoctorDetails(int id, UpdatestatusDTO dto)
        {
            var doctor = await _context.doctorDetails.FindAsync(id);
            doctor.RequestStatus = dto.RequestStatus;

            await _context.SaveChangesAsync();
            return doctor;
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

        public async Task<DoctorDetails> RegisterDoctorAsync(DoctorDetails doctor, IFormFile imageFile)
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

                doctor.DoctorImage = fileName;
                doctor.DoctorPassword = Encrypt(doctor.DoctorPassword);
                _context.doctorDetails.Add(doctor);
                _context.SaveChanges();

                return doctor;
            }
            catch (Exception ex)
            {
    
                throw new Exception("Error occurred while posting the room.", ex);
            }

        }

        public async Task<DoctorDetails> PutDoctorProfile(int id, [FromForm] ProfileUpdateDTO dto, IFormFile imageFile)
        {
            var existingDoctor = await _context.doctorDetails.FindAsync(id);

                if (existingDoctor == null)
                {
                    throw new ArgumentException("Doctor not found");
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                    if (!string.IsNullOrEmpty(existingDoctor.DoctorImage))
                    {
                        var existingFilePath = Path.Combine(uploadsFolder, existingDoctor.DoctorImage);
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

                    dto.DoctorImage = fileName;
                }
                else
                {
                    dto.DoctorImage = existingDoctor.DoctorImage;
                }

                existingDoctor.Specialization = dto.Specialization;
                existingDoctor.ExperienceYears = dto.ExperienceYears;
                existingDoctor.Phone = dto.Phone;
                existingDoctor.Availability = dto.Availability;
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
}
