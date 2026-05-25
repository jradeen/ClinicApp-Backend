using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.API
{
    // Controllers/StaffController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet("clinic")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> GetClinicStaff()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var staff = await _staffService.GetByOwnerIdAsync(ownerId);
                return Ok(staff);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> GetById(int id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var staff = await _staffService.GetByIdAsync(id, ownerId);
                return Ok(staff);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> Create([FromBody] CreateStaffDto dto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var staff = await _staffService.CreateAsync(dto, ownerId);
                return CreatedAtAction(nameof(GetById), new { id = staff.Id }, staff);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStaffDto dto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var staff = await _staffService.UpdateAsync(id, dto, ownerId);
                return Ok(staff);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> Delete(int id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _staffService.DeleteAsync(id,ownerId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
