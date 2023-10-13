using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models.Library;

namespace LibrarySystem.Data
{
    public partial class LibraryContext : DbContext
    {
        public LibraryContext()
        {
        }

        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LibrarySystem.Models.Library.Signin>()
              .Property(p => p.time_in)
              .HasColumnType("datetime");

            builder.Entity<LibrarySystem.Models.Library.Signin>()
              .Property(p => p.time_out)
              .HasColumnType("datetime");
            this.OnModelBuilding(builder);
        }

        public DbSet<LibrarySystem.Models.Library.Book> Books { get; set; }

        public DbSet<LibrarySystem.Models.Library.Category> Categories { get; set; }

        public DbSet<LibrarySystem.Models.Library.RentHistory> RentHistories { get; set; }

        public DbSet<LibrarySystem.Models.Library.Signin> Signins { get; set; }

        public DbSet<LibrarySystem.Models.Library.User> Users { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    
    }
}