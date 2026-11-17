namespace InteractiveStories.Data.Entities;

public class ReadingProgress
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public int StoryId { get; set; }

    public Story Story { get; set; } = null!;

    public int CurrentSceneId { get; set; }

    public Scene CurrentScene { get; set; } = null!;

    public bool IsCompleted { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
