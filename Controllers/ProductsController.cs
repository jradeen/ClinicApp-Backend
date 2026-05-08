using System.Security.Claims;
using ClinicApp.API.DTOs.Product;
using ClinicApp.API.Interfaces.IProduct;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Roles = "ClinicOwner")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto productDto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _productService.CreateAsync(productDto, ownerId);
            if (result == null) return NotFound("Clinic does not exists");

            return CreatedAtAction(nameof(GetProductByClinicId), new { clinicId = result.ClinicId }, result);
        }

        [HttpGet("clinic/{clinicId:int}")]
        public async Task<IActionResult> GetProductByClinicId(int clinicId)
        {
            return Ok(await _productService.GetByClinicAsync(clinicId));
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
        
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> Delete(int id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _productService.DeleteProductAsync(id, ownerId);
                if (!result)
                    return NotFound();

                return Ok("Product Deleted successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ClinicOwner")]
        public async Task<IActionResult> Update(int id, UpdateProductDto updateDto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _productService.UpdateProductAsync(id, updateDto, ownerId);
                if (result == null)
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
