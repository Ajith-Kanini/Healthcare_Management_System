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

        // GET: api/DoctorDetails
        [Authorize(Roles = "Doctor,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctorDetails()
        {
            var doctorDetails = await _repository.GetAllDoctorDetailsAsync();
            var doctorDTOs = doctorDetails.Select(d => new DoctorDTO
            {
                DoctoName=d.DoctoName,
                Email=d.Email,
                DoctorPassword=d.DoctorPassword,
            }).ToList();

            return doctorDTOs;
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

        // POST: api/DoctorDetails
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostDoctorDetails(DoctorDTO doctorDTO)
        {
            var doctorDetails = new DoctorDetails
            {
                DoctoName = doctorDTO.DoctoName,
                Email = doctorDTO.Email,
                DoctorPassword = Encrypt(doctorDTO.DoctorPassword)
                // Map other properties as needed
            };

            await _repository.CreateDoctorDetailsAsync(doctorDetails);

            return CreatedAtAction("GetDoctorDetails", new { id = doctorDetails.DoctorId }, doctorDTO);
        }

        // PUT: api/DoctorDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctorDetails(int id, DoctorDTO doctorDTO)
        {
  
            

            return NoContent();
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
