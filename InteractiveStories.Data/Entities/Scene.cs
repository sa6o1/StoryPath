namespace InteractiveStories.Data.Entities;

public class Scene
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public bool IsEnding { get; set; }

    public int ChapterId { get; set; }

    public Chapter Chapter { get; set; } = null!;

    // Изборите, които започват от текущата сцена.
    public ICollection<Choice> OutgoingChoices { get; set; } = new List<Choice>();

    // Изборите от други сцени, които водят към текущата.
    public ICollection<Choice> IncomingChoices { get; set; } = new List<Choice>();
}
