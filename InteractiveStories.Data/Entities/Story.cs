namespace InteractiveStories.Data.Entities;

public class Story
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsPublished { get; set; }

    public string AuthorId { get; set; } = string.Empty;

    public ApplicationUser Author { get; set; } = null!;

    public int GenreId { get; set; }

    public Genre Genre { get; set; } = null!;

    // Новата история още няма начална сцена, затова полето допуска null.
    public int? StartSceneId { get; set; }

    public Scene? StartScene { get; set; }

    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}
