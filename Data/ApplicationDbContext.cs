using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace aspnetcore_storagemanager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<FileStorage> FileStorages { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<FileStorage>().HasKey(f => f.FileId);
            base.OnModelCreating(builder);
        }
    }
}