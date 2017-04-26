using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Sample.Models
{
    public class SampleDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Todo> todos { get; set; }

        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            // shadow properties
            builder.Entity<Todo>().Property<DateTime>("UpdatedTimestamp");

            builder.HasPostgresExtension("uuid-ossp");

            base.OnModelCreating(builder);

        }
    }

    public class Todo
    {
        [Key]
        public Guid Id { get; set; }
        
        public string TodoItem {get; set;}

    }
}