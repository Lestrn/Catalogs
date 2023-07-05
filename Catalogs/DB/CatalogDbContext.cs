using Catalogs.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace Catalogs.DB
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<CatalogDataModel> Data { get; set; }
        public DbSet<CatalogModel> Catalog { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatalogModel>()
                .HasData(
                    new CatalogModel {Id = Guid.NewGuid(), CatalogName = "", CatalogRoute = ""}
                );
            base.OnModelCreating(modelBuilder);
        }

    }
}
