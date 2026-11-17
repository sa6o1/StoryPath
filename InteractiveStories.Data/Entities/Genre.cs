namespace InteractiveStories.Data.Entities;

public class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Story> Stories { get; set; } = new List<Story>();
}
