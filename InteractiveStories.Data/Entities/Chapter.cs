namespace InteractiveStories.Data.Entities;

public class Chapter
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int OrderNumber { get; set; }

    public int StoryId { get; set; }

    public Story Story { get; set; } = null!;

    public ICollection<Scene> Scenes { get; set; } = new List<Scene>();
}
