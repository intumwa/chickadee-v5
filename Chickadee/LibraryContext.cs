using Microsoft.EntityFrameworkCore;

namespace Chickadee
{
    public class LibraryContext : DbContext
    {
        public DbSet<Domain> Domain { get; set; }
        public DbSet<Config> Config { get; set; }
        public DbSet<WebPage> WebPage { get; set; }
        public DbSet<WebPageVisit> WebPageVisit { get; set; }
        public DbSet<DomChange> DomChange { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=127.0.0.1;database=chickadee;user=root;password=;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Domain>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasIndex(e => e.Url).IsUnique();
            });

            modelBuilder.Entity<Config>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasIndex(e => e.UID).IsUnique();
            });

            modelBuilder.Entity<WebPage>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasIndex(e => e.Url).IsUnique();
                entity.Property(e => e.isVisited).IsRequired();
                entity.Property(e => e.didRequestSucceed).IsRequired();
                entity.Property(e => e.isUaComputed).IsRequired();
            });

            modelBuilder.Entity<WebPageVisit>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Url).IsRequired();
                entity.Property(e => e.ConfigurationUid).IsRequired();
                entity.Property(e => e.VisitTime).IsRequired();
                entity.Property(e => e.IsDomProcessed).IsRequired();
                entity.Property(e => e.FilePath).IsRequired();
                entity.HasOne(d => d.WebPage).WithMany(p => p.WebPageVisits);
            });

            modelBuilder.Entity<DomChange>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Ua1NodesCount).IsRequired();
                entity.Property(e => e.Ua2NodesCount).IsRequired();
                entity.HasOne(d => d.Ua1Visit).WithMany(p => p.ua1DomChanges);
                entity.HasOne(d => d.Ua2Visit).WithMany(p => p.ua2DomChanges);
            });
        }
    }
}
