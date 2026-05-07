using System.Security.Claims;
using System.Text.Json.Serialization;
using ClinicApp.API.DTOs.Order;
using ClinicApp.API.Interfaces.IClinic;
using ClinicApp.API.Interfaces.IOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IClinicRepository _clinicRepo;
        public OrdersController(IOrderService orderService, IClinicRepository clinicRepo)
        {
            _orderService = orderService;
            _clinicRepo = clinicRepo;
        }
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create(CreateOrderDto createOrderDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _orderService.CreateAsync(createOrderDto, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }
        [HttpGet("clinic")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> GetClinicOrders()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var clinic = await _clinicRepo.GetByOwnerIdAsync(ownerId);
            if (clinic == null) return NotFound("You do not have a clinic registered");

            var orders = await _orderService.GetClinicOrdersAsync(clinic.Id);
            return Ok(orders);
        }
        [HttpPatch("{id:int}/Status")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _orderService.UpdateStatusAsync(id, status, ownerId);
                if (!result) return NotFound();

                return Ok(new { message = $"Order status updated to {status}" });
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
