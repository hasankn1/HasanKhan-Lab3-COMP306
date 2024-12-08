using HasanKhan_Lab3_COMP306.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HasanKhan_Lab3_COMP306;

namespace HasanKhan_Lab3_COMP306.DbData
{
    public class Lab3Comp306DbContext: DbContext
    {
        public Lab3Comp306DbContext(DbContextOptions<Lab3Comp306DbContext> options)
            : base(options)
        {
        }
        // public DbSet<Movies> Movies { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // modelBuilder.Entity<Movies>().HasKey(m => m.MovieID);
            modelBuilder.Entity<User>()
        .HasKey(m => m.UserEmail);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}
