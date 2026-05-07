using System.Security.Claims;
using ClinicApp.API.DTOs.Booking;
using ClinicApp.API.Interfaces.IBooking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create(CreateBookingDto bookingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _bookingService.CreateAsync(bookingDto, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _bookingService.GetUserBookingsAsync(userId));

        }
        [HttpGet("clinic")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> GetClinicBookings()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _bookingService.GetClinicBookingsAsync(ownerId));
        }
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _bookingService.UpdateStatusAsync(id, status, ownerId);
                if (!result) return NotFound();

                return Ok(new { message = $"Booking status updated to {status}" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
