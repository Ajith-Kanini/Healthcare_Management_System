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
using System.Numerics;

namespace DoctorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorDetailsController : ControllerBase
    {
        private readonly IDoctorDetailsRepository _repository;
        public DoctorDetailsController(IDoctorDetailsRepository repository)
        {
            _repository = repository;
        }


        // [Authorize(Roles = "Doctor,Admin,User")]
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

        // [Authorize(Roles = "Admin,Doctor,User")]
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
                DoctoName = doctorDetails.DoctoName,
                Email = doctorDetails.Email,
                DoctorPassword = doctorDetails.DoctorPassword,
            };

            return doctorDTO;
        }

        // [Authorize(Roles = "Admin,Doctor,User")]
        [HttpGet("FullDetails/{id}")]
        public async Task<ActionResult<DoctorDetails>> GetFullDoctorDetailsByIdAsync(int id)
        {
            var doctorDetails = await _repository.GetDoctorDetailsByIdAsync(id);

            return doctorDetails;
        }

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


        [Authorize(Roles = "Doctor")]
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
        [HttpPut("updateStatus/{id}")]

        public async Task<DoctorDetails> PutDoctorDetails(int id, UpdatestatusDTO dtor)
        {
            return await _repository.PutDoctorDetails(id, dtor);
        }
        [HttpPost("Register")]
        public async Task<ActionResult<DoctorDetails>> PostCourse([FromForm] DoctorDetails patient, IFormFile imageFile)
        {

            var createdCourse = await _repository.RegisterDoctorAsync(patient, imageFile);

            return CreatedAtAction("RegisterCourse", new { id = createdCourse.DoctorId }, createdCourse);
        }
        // [Authorize(Roles = "Doctor")]
        [HttpPut("Updateprofile/{id}")]
        public async Task<ActionResult<DoctorDetails>> PutDoctorProfile(int id, [FromForm] ProfileUpdateDTO dto, IFormFile imageFile)
        {
            try
            {
                var updatedDoc = await _repository.PutDoctorProfile(id, dto, imageFile);
                return (updatedDoc);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }

        }
        private string Encrypt(string password)
        {
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
