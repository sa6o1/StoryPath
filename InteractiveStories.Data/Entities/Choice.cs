namespace InteractiveStories.Data.Entities;

public class Choice
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public int OrderNumber { get; set; }

    public int FromSceneId { get; set; }

    public Scene FromScene { get; set; } = null!;

    public int ToSceneId { get; set; }

    public Scene ToScene { get; set; } = null!;
}
