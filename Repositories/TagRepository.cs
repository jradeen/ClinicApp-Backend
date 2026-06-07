using ClinicApp.API.Data;
using ClinicApp.API.Repositories;
using Microsoft.EntityFrameworkCore;
public class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;
    public TagRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Tag>> GetAllTagsAsync(string? category)
    {
        return await _context.Tags.Where(t => category == null || t.Category == category).ToListAsync();
    }
}