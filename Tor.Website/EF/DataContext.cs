using Microsoft.EntityFrameworkCore;
using Tor.Website.Models.DBO;

namespace Tor.Website.EF
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticlePreview> Previews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<TorImage> Images { get; set; }
    }
}
