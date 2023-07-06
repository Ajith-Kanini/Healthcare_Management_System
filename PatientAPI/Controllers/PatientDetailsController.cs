using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using PatientAPI.Models;
using PatientAPI.Models.DTO;
using PatientAPI.Repository;

namespace PatientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientDetailsController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientDetailsController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpGet]
       //[Authorize(Roles = "Admin,Patient")]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetpatientDetails()
        {
            var patients = await _patientRepository.GetPatients();
            return Ok(patients);
        }
        //[Authorize(Roles = "Admin,Patient")]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<PatientDetails>>> GetAllpatientDetails()
        {
            var patients = await _patientRepository.GetAllPatients();
            return Ok(patients);
        }
        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDetails>> GetPatientDetails(int id)
        {
            var patient = await _patientRepository.GetPatient(id);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatientDetails(int id, PatientDTO patientDto)
        {
            var updated = await _patientRepository.UpdatePatient(id, patientDto);
            if (!updated)
                return NotFound();

            return NoContent();
        }
        [HttpPost("Register")]
        public async Task<ActionResult<PatientDetails>> PostCourse([FromForm] PatientDetails patient, IFormFile imageFile)
        {
            var createdCourse = await _patientRepository.RegisterPatientAsync(patient, imageFile);

            return CreatedAtAction("RegisterCourse", new { id = createdCourse.PatientId }, createdCourse);
        }

     
        [HttpPost]
        public async Task<ActionResult<PatientDTO>> PostPatientDetails(PatientDTO patientDto)
        {
            var id = await _patientRepository.CreatePatient(patientDto);
            return CreatedAtAction(nameof(GetPatientDetails), new { id }, patientDto);
        }
        [Authorize(Roles = "User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientDetails(int id)
        {
            var deleted = await _patientRepository.DeletePatient(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        [Authorize(Roles = "User")]
        [HttpPut("Updateprofile/{id}")]
        public async Task<ActionResult<PatientDetails>> PutPatientProfile(int id, [FromForm] PatientProfileDTO dto, IFormFile imageFile)
        {
            try
            {
                var updatedDoc = await _patientRepository.PutPatientProfile(id, dto, imageFile);
                return (updatedDoc);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }

        }
    }

}
