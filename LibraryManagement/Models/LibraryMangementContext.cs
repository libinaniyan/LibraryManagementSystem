using Microsoft.EntityFrameworkCore;

namespace LibraryMangement.Models
{
    public class LibraryMangementContext:DbContext
    {
        public LibraryMangementContext(DbContextOptions<LibraryMangementContext> options) : base(options)
        {

        }
        public DbSet<Members> members { get; set; }
        public DbSet<Books> books { get; set; }
        public DbSet<BorrowedBooks> borrowedBooks { get; set; }
        public DbSet<Waitlists> waitlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define foreign key relationships
            modelBuilder.Entity<BorrowedBooks>()
                .HasOne(bb => bb.Member)
                .WithMany()
                .HasForeignKey(bb => bb.MemberId);

            modelBuilder.Entity<BorrowedBooks>()
                .HasOne(bb => bb.Book)
                .WithMany()
                .HasForeignKey(bb => bb.BookId);

            modelBuilder.Entity<Waitlists>()
                .HasOne(w => w.Member)
                .WithMany()
                .HasForeignKey(w => w.MemberId);

            modelBuilder.Entity<Waitlists>()
                .HasOne(w => w.Book)
                .WithMany()
                .HasForeignKey(w => w.BookId);
        }

    }
}
