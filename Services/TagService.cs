using ClinicApp.API.Models;
using ClinicApp.API.Repositories;
using Microsoft.IdentityModel.Tokens;

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
        var validCategory = new[] { "Clinic", "MedicalService", "Product"};
        if (!string.IsNullOrEmpty(category) &&!validCategory.Contains(category, StringComparer.OrdinalIgnoreCase)) throw new Exception("Invalid category it must be : { Clinic, MedicalService, Product}");

        return await _tagRepo.GetAllTagsAsync(category);
    }
}