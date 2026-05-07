using System.Security.Claims;
using ClinicApp.API.DTOs.Clinic;
using ClinicApp.API.Interfaces.IClinic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicsController : ControllerBase
    {
        private readonly IClinicService _clinicService;
        public ClinicsController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        [HttpPost]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> Create(CreateClinicDto clinicDto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var clinic = await _clinicService.CreateClinicAsync(clinicDto, ownerId);
            if (clinic == null) return BadRequest("You already own a clinic.");

            return Ok(clinic);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var clinics = await _clinicService.GetAllAsync();
            return Ok(clinics);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var clinic = await _clinicService.GetByIdAsync(id);
            if (clinic == null) return NotFound();

            return Ok(clinic);
        }

    }
}
