using ClinicApp.API.Models;
using ClinicApp.API.Repositories;

namespace ClinicApp.API.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepo;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepo = tagRepository;
    }
    public async Task<List<Tag>> GetAllTagsAsync(string? category)
    {
        return await _tagRepo.GetAllTagsAsync(category);
    }
}