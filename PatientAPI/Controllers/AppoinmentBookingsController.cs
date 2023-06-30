using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientAPI.Models;
using PatientAPI.Repository;

namespace PatientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppoinmentBookingsController : ControllerBase
    {
        private readonly IAppoinmentBookingRepository _repository;

        public AppoinmentBookingsController(IAppoinmentBookingRepository repository)
        {
            _repository = repository;
        }

        // GET: api/AppoinmentBookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppoinmentBooking>>> GetappoinmentBookings()
        {
            var appoinmentBookings = await _repository.GetAppoinmentBookingsAsync();
            return Ok(appoinmentBookings);
        }

        // GET: api/AppoinmentBookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppoinmentBooking>> GetAppoinmentBooking(int id)
        {
            var appoinmentBooking = await _repository.GetAppoinmentBookingByIdAsync(id);

            if (appoinmentBooking == null)
            {
                return NotFound();
            }

            return appoinmentBooking;
        }

        // PUT: api/AppoinmentBookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppoinmentBooking(int id, AppoinmentBooking appoinmentBooking)
        {
            if (id != appoinmentBooking.AppointmentId)
            {
                return BadRequest();
            }

            try
            {
                await _repository.UpdateAppoinmentBookingAsync(appoinmentBooking);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppoinmentBookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AppoinmentBookings
        [HttpPost]
        public async Task<ActionResult<AppoinmentBooking>> PostAppoinmentBooking(AppoinmentBooking appoinmentBooking)
        {
            await _repository.CreateAppoinmentBookingAsync(appoinmentBooking);

            return CreatedAtAction("GetAppoinmentBooking", new { id = appoinmentBooking.AppointmentId }, appoinmentBooking);
        }

        // DELETE: api/AppoinmentBookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppoinmentBooking(int id)
        {
            var appoinmentBooking = await _repository.GetAppoinmentBookingByIdAsync(id);
            if (appoinmentBooking == null)
            {
                return NotFound();
            }

            await _repository.DeleteAppoinmentBookingAsync(appoinmentBooking);

            return NoContent();
        }

        private bool AppoinmentBookingExists(int id)
        {
            return _repository.AppoinmentBookingExists(id);
        }
    }
}
