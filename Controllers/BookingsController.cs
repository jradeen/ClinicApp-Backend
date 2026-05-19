using System.Data;
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
        [HttpPatch("{id:int}/cancel")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var result = await _bookingService.CancelBooking(id, userId);
                if (!result) return NotFound();

                return Ok(new { message = $"Booking Cancelled" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpGet("available-slots")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int medicalServiceId, [FromQuery] DateOnly date)
        {
            try
            {
                var slots = await _bookingService.GetAvailableSlotsAsync(medicalServiceId, date);
                return Ok(slots);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
