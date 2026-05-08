using System.Security.Claims;
using ClinicApp.API.DTOs.MedicalService;
using ClinicApp.API.Interfaces.IMedicalService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalServicesController : ControllerBase
    {
        private readonly IMedicalServiceService _medicalService;
        public MedicalServicesController(IMedicalServiceService medicalService)
        {
            _medicalService = medicalService;
        }

        [Authorize(Roles = "ClinicOwner")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateMedicalServiceDto medicalServiceDto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _medicalService.CreateAsync(medicalServiceDto, ownerId);
            if (result == null) return NotFound("Owner does not have a clinic yet");

            return CreatedAtAction(nameof(GetMedicalServiceByClinicId), new { clinicId = result.ClinicId }, result);
        }

        [HttpGet("clinic/{clinicId:int}")]
        public async Task<IActionResult> GetMedicalServiceByClinicId(int clinicId)
        {
            return Ok(await _medicalService.GetByClinicAsync(clinicId));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var medicalServices = await _medicalService.GetAllAsync();
            return Ok(medicalServices);
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> Delete(int id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _medicalService.DeleteMedicalServiceAsync(id, ownerId);
                if (!result)
                    return NotFound();

                return Ok("Medical service Deleted successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> Update(int id, UpdateMedicalServiceDto updateDto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _medicalService.UpdateMedicalServiceAsync(id,updateDto, ownerId);
                if (result==null)
                    return NotFound();

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            } 
        }

    }
}
