using ClinicApp.API.Models;

namespace ClinicApp.API.Services;

public interface ITagService
{
    Task<List<Tag>> GetAllTagsAsync(string? category);
}
