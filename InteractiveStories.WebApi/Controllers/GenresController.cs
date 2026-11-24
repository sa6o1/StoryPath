using InteractiveStories.Core.Models;
using InteractiveStories.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace InteractiveStories.WebApi.Controllers;

[ApiController]
[Route("api/genres")]
public class GenresController : ControllerBase
{
    private readonly GenreService genreService;

    public GenresController(GenreService genreService)
    {
        this.genreService = genreService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GenreModel>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await genreService.GetAllAsync(cancellationToken));
    }
}
