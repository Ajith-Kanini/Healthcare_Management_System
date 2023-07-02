using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DoctorAPI.Models.DTO;
using DoctorAPI.Models;
using DoctorAPI.Repository;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Prometheus.DotNetRuntime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace DoctorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorDetailsController : ControllerBase
    {
        private readonly IDoctorDetailsRepository _repository;
        private readonly DoctorDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DoctorDetailsController(DoctorDbContext context,IDoctorDetailsRepository repository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/DoctorDetails
       // [Authorize(Roles = "Doctor,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDetails>>> GetDoctorDetails()
        {
            var courses = await _repository.GetAllDoctorDetailsAsync();

            if (courses == null)
            {
                return NotFound();
            }

            return Ok(courses);

        }

        // GET: api/DoctorDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctorDetails(int id)
        {
            var doctorDetails = await _repository.GetDoctorDetailsByIdAsync(id);
            if (doctorDetails == null)
            {
                return NotFound();
            }

            var doctorDTO = new DoctorDTO
            {
                DoctoName =doctorDetails.DoctoName,
                Email = doctorDetails.Email,
                DoctorPassword = doctorDetails.DoctorPassword,
            };

            return doctorDTO;
        }

        [HttpGet("FullDetails/{id}")]
        public async Task<ActionResult<DoctorDetails>> GetFullDoctorDetailsByIdAsync(int id)
        {
            var doctorDetails = await _repository.GetDoctorDetailsByIdAsync(id);
            
            return doctorDetails;
        }
        // POST: api/DoctorDetails
       // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostDoctorDetails(DoctorDTO doctorDTO)
        {
            var doctorDetails = new DoctorDetails
            {
                DoctoName = doctorDTO.DoctoName,
                Email = doctorDTO.Email,
                DoctorPassword = Encrypt(doctorDTO.DoctorPassword)
            };

            await _repository.CreateDoctorDetailsAsync(doctorDetails);

            return CreatedAtAction("GetDoctorDetails", new { id = doctorDetails.DoctorId }, doctorDTO);
        }

        // PUT: api/DoctorDetails/5
        [HttpPut("updateStatus/{id}")]
 
        public async Task<DoctorDetails> PutDoctorDetails(int id, UpdatestatusDTO dtor)
        {
                return await _repository.PutDoctorDetails(id, dtor);
        }

        // DELETE: api/DoctorDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctorDetails(int id)
        {
            var doctorDetails = await _repository.GetDoctorDetailsByIdAsync(id);
            if (doctorDetails == null)
            {
                return NotFound();
            }

            await _repository.DeleteDoctorDetailsAsync(id);

            return NoContent();
        }
        [HttpPost("Register")]
        public async Task<ActionResult<DoctorDetails>> PostCourse([FromForm] DoctorDetails patient, IFormFile imageFile)
        {
            var createdCourse = await _repository.RegisterDoctorAsync(patient, imageFile);

            return CreatedAtAction("RegisterCourse", new { id = createdCourse.DoctorId }, createdCourse);
        }
        [HttpPut("UpdatedProfiles/{id}")]
        public async Task<IActionResult> PutDoctorProfile(int id, ProfileUpdateDTO dto, IFormFile imageFile)
        {
            var updatedDoctor = await _repository.PutDoctorProfile(id, dto, imageFile);
            return Ok(updatedDoctor);
        }

        public async Task<DoctorDetails> PutDoctorProfiles(int id, ProfileUpdateDTO dto, IFormFile imageFile)
        {
            var doctor = await _context.doctorDetails.FindAsync(id);

            if (doctor == null)
            {
                throw new ArgumentException("Doctor not found");
            }

            doctor.Availability = dto.Availability;
            doctor.Address = dto.Address;
            doctor.State = dto.State;
            doctor.ExperienceYears = dto.ExperienceYears;
            doctor.Specialization = dto.Specialization;
            doctor.Phone = dto.Phone;

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

                    // Delete the old image file
                   /* if (!string.IsNullOrEmpty(doctor.DoctorImage))
                    {
                        var oldFilePath = Path.Combine(uploadsFolder, doctor.DoctorImage);
                        File.Delete(oldFilePath);
                    }
*/
                    doctor.DoctorImage = fileName;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error occurred while updating the doctor's profile.", ex);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return doctor;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while saving the updated doctor profile.", ex);
            }
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
