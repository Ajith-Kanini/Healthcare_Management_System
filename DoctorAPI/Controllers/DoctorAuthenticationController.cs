using AdminAPI.Models.DTO;
using DoctorAPI.Models;
using DoctorAPI.Models.DTO;
using PatientAPI.Models;
using PatientAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DoctorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorAuthenticationController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly DoctorDbContext _context;
        private readonly PatientDbContext _patient;
        private const string DoctorRole = "Doctor";
        /*private const string AdminRole = "Admin";
        private const string UserRole = "User";*/

        public DoctorAuthenticationController(IConfiguration config, DoctorDbContext context,PatientDbContext patient)
        {
            _configuration = config;
            _context = context;
            _patient = patient;
        }

       


        [HttpPost("Doctor")]
        public async Task<IActionResult> Post(DoctorDTO _userData)
        {
            if (_userData != null && _userData.Email != null && _userData.DoctorPassword != null)
            {
                var user = await GetDoctor(_userData.Email, Encrypt(_userData.DoctorPassword));

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),

                         new Claim("Email", user.Email),
                        new Claim("DoctorPassword",user.DoctorPassword),
                        new Claim(ClaimTypes.Role,DoctorRole),

                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                       signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }

        }
        private async Task<DoctorDetails> GetDoctor(string email, string password)
        {
            return await _context.doctorDetails.FirstOrDefaultAsync(u => u.Email == email && u.DoctorPassword == password);
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
