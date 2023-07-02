using DoctorAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Humanizer;
using DoctorAPI.Models.DTO;

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

                _context.doctorDetails.Add(doctor);
                _context.SaveChanges();

                return doctor;
            }
            catch (Exception ex)
            {
                // Rethrow the exception with additional information
                throw new Exception("Error occurred while posting the room.", ex);
            }

        }

        public async Task<DoctorDetails> PutDoctorDetails(int id, UpdatestatusDTO dto)
        {
            var doctor = await _context.doctorDetails.FindAsync(id);
            doctor.RequestStatus = dto.RequestStatus;

            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task<DoctorDetails> PutDoctorProfile(int id, ProfileUpdateDTO dto, IFormFile imageFile)
        {
            var doctor = await _context.doctorDetails.FindAsync(id);

            if (doctor == null)
            {
                throw new ArgumentException("Doctor not found");
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    if (!string.IsNullOrEmpty(doctor.DoctorImage))
                    {
                        var oldFilePath = Path.Combine(uploadsFolder, doctor.DoctorImage);
                        File.Delete(oldFilePath);
                    }

                    doctor.DoctorImage = fileName;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error occurred while updating the doctor.", ex);
                }
            }
            doctor.Availability = dto.Availability;
           doctor.Address = dto.Address;
            doctor.State = dto.State;
            doctor.ExperienceYears= dto.ExperienceYears;
            doctor.Specialization= dto.Specialization;
            doctor.Phone= dto.Phone;

            try
            {
                await _context.SaveChangesAsync();
                return doctor;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while saving the updated doctor details.", ex);
            }
        }
    }
}
