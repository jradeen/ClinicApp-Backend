using ClinicApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ClinicApp.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTags([FromQuery] string? category)
        {
            try
            {
                var tags = await _tagService.GetAllTagsAsync(category);
                if (tags.IsNullOrEmpty()) return NotFound();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
