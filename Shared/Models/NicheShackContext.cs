﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models
{
    public class NicheShackContext : IdentityDbContext<Customer>
    {
        public NicheShackContext(DbContextOptions<NicheShackContext> options)
            : base(options)
        {
        }

        // Tables
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Filter> Filters { get; set; }
        public virtual DbSet<FilterOption> FilterOptions { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<ListCollaborator> ListCollaborators { get; set; }
        public virtual DbSet<ListProduct> ListProducts { get; set; }
        public virtual DbSet<Niche> Niches { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }
        public virtual DbSet<PriceIndex> PriceIndices { get; set; }
        public virtual DbSet<PriceRange> PriceRanges { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductContent> ProductContent { get; set; }
        public virtual DbSet<ProductFilter> ProductFilters { get; set; }
        public virtual DbSet<ProductMedia> ProductMedia { get; set; }
        public virtual DbSet<ProductOrder> ProductOrders { get; set; }
        public virtual DbSet<ProductPricePoint> ProductPricePoints { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");



            // Customers
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable(name: "Customers");
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });








            // ListProducts
            modelBuilder.Entity<ListProduct>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.CollaboratorId })
                    .HasName("PK_ListProducts");
            });



            // OrderProducts
            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.OrderId })
                    .HasName("PK_OrderProducts");
            });







            // ProductFilters
            modelBuilder.Entity<ProductFilter>(entity =>
            {
                entity.HasKey(x => new { x.ProductId, x.FilterOptionId })
                .HasName("PK_ProductFilters");
            });


            // ProductMedia
            modelBuilder.Entity<ProductMedia>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.MediaId })
                    .HasName("PK_ProductMedia");
            });







            // RefreshTokens
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.CustomerId })
                    .HasName("PK_RefreshTokens");
            });
        }
    }
}
