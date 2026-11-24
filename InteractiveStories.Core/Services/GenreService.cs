using InteractiveStories.Core.Models;
using InteractiveStories.Data;
using Microsoft.EntityFrameworkCore;

namespace InteractiveStories.Core.Services;

public class GenreService
{
    private readonly ApplicationDbContext context;

    public GenreService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<List<GenreModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Genres
            .AsNoTracking()
            .OrderBy(genre => genre.Id)
            .Select(genre => new GenreModel { Id = genre.Id, Name = genre.Name })
            .ToListAsync(cancellationToken);
    }
}
