using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WireMess.Models.Entities;
using WireMess.Models.Enums;

namespace WireMess.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<UserConversation> UserConversations { get; set; }
        public DbSet<ConversationType> ConversationTypes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Message>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<Attachment>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<Conversation>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<UserConversation>().HasQueryFilter(uc => !uc.IsDeleted);

            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<ConversationType>().HasData(
            new ConversationType
            {
                Id = (int)ConversationTypeEnum.Direct,
                TypeName = nameof(ConversationTypeEnum.Direct),
                Description = "One-on-one chat",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new ConversationType
            {
                Id = (int)ConversationTypeEnum.Group,
                TypeName = nameof(ConversationTypeEnum.Group),
                Description = "Multi-user chat",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity &&
                (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var entity = (BaseEntity)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
