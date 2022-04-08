using Microsoft.EntityFrameworkCore;

namespace dotnetProj.Models
{
    public partial class MyDatabaseContext : DbContext
    {
        public MyDatabaseContext()
        {
        }

        public MyDatabaseContext(DbContextOptions<MyDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Person> People { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var currentDir = Directory.GetCurrentDirectory();
                optionsBuilder.UseSqlite($"datasource={currentDir}\\MyDatabase.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
