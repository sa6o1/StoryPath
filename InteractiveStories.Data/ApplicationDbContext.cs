using InteractiveStories.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InteractiveStories.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<Scene> Scenes => Set<Scene>();
    public DbSet<Choice> Choices => Set<Choice>();
    public DbSet<ReadingProgress> ReadingProgresses => Set<ReadingProgress>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Конфигурира седемте стандартни таблици на Identity.
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers", "dbo");
            entity.Property(x => x.Id).HasMaxLength(450);
            entity.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Bio).HasMaxLength(1000);
        });

        // Дължините съответстват на съществуващия SQL скрипт.
        builder.Entity<IdentityRole>().ToTable("AspNetRoles", "dbo");
        builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(450);
        builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "dbo");
        builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "dbo");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "dbo");

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("AspNetUserLogins", "dbo");
            entity.Property(x => x.LoginProvider).HasMaxLength(128);
            entity.Property(x => x.ProviderKey).HasMaxLength(128);
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("AspNetUserTokens", "dbo");
            entity.Property(x => x.LoginProvider).HasMaxLength(128);
            entity.Property(x => x.Name).HasMaxLength(128);
        });

        builder.Entity<Genre>(entity =>
        {
            entity.ToTable("Genres", "dbo");
            entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
        });

        builder.Entity<Story>(entity =>
        {
            entity.ToTable("Stories", "dbo");
            entity.Property(x => x.Title).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnType("datetime2");

            entity.HasOne(x => x.Author)
                .WithMany(x => x.Stories)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.Genre)
                .WithMany(x => x.Stories)
                .HasForeignKey(x => x.GenreId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.StartScene)
                .WithMany()
                .HasForeignKey(x => x.StartSceneId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Chapter>(entity =>
        {
            entity.ToTable("Chapters", "dbo");
            entity.Property(x => x.Title).HasMaxLength(150).IsRequired();
            entity.HasIndex(x => new { x.StoryId, x.OrderNumber }).IsUnique();

            entity.HasOne(x => x.Story)
                .WithMany(x => x.Chapters)
                .HasForeignKey(x => x.StoryId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Scene>(entity =>
        {
            entity.ToTable("Scenes", "dbo");
            entity.Property(x => x.Title).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();

            entity.HasOne(x => x.Chapter)
                .WithMany(x => x.Scenes)
                .HasForeignKey(x => x.ChapterId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Choice>(entity =>
        {
            entity.ToTable("Choices", "dbo");
            entity.Property(x => x.Text).HasMaxLength(300).IsRequired();
            entity.HasIndex(x => new { x.FromSceneId, x.OrderNumber }).IsUnique();

            // Две отделни връзки към Scenes, без каскадно изтриване.
            entity.HasOne(x => x.FromScene)
                .WithMany(x => x.OutgoingChoices)
                .HasForeignKey(x => x.FromSceneId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.ToScene)
                .WithMany(x => x.IncomingChoices)
                .HasForeignKey(x => x.ToSceneId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<ReadingProgress>(entity =>
        {
            entity.ToTable("ReadingProgresses", "dbo");
            entity.Property(x => x.UpdatedAt).HasColumnType("datetime2");
            entity.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique();

            entity.HasOne(x => x.User)
                .WithMany(x => x.ReadingProgresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.Story)
                .WithMany()
                .HasForeignKey(x => x.StoryId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.CurrentScene)
                .WithMany()
                .HasForeignKey(x => x.CurrentSceneId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
