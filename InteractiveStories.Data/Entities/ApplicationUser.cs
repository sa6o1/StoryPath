using Microsoft.AspNetCore.Identity;

namespace InteractiveStories.Data.Entities;

// IdentityUser съдържа Id, Email, UserName и защитения хеш на паролата.
public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public ICollection<Story> Stories { get; set; } = new List<Story>();

    public ICollection<ReadingProgress> ReadingProgresses { get; set; }
        = new List<ReadingProgress>();
}
