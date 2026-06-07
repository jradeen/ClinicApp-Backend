namespace ClinicApp.API.Repositories;

public interface ITagRepository
{
    Task<List<Tag>> GetAllTagsAsync(string? category);
}