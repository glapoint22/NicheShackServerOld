using Microsoft.AspNetCore.Identity;
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


        public virtual DbSet<Email> Emails { get; set; }


        public virtual DbSet<Filter> Filters { get; set; }
        public virtual DbSet<FilterOption> FilterOptions { get; set; }


        public virtual DbSet<Keyword> Keywords { get; set; }

        public virtual DbSet<KeywordSearchVolume> KeywordSearchVolumes { get; set; }

        public virtual DbSet<LeadPage> LeadPages { get; set; }


        public virtual DbSet<LeadPageEmail> LeadPageEmails { get; set; }




        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<ListCollaborator> ListCollaborators { get; set; }
        public virtual DbSet<ListProduct> ListProducts { get; set; }



        public virtual DbSet<Media> Media { get; set; }



        public virtual DbSet<Niche> Niches { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationText> NotificationText { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }


        public virtual DbSet<Page> Pages { get; set; }


        public virtual DbSet<PageDisplayTypeId> PageDisplayTypeIds { get; set; }


        public virtual DbSet<PriceIndex> PriceIndices { get; set; }
        public virtual DbSet<PriceRange> PriceRanges { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductContent> ProductContent { get; set; }


        public virtual DbSet<ProductEmail> ProductEmails { get; set; }




        public virtual DbSet<ProductFilter> ProductFilters { get; set; }
        public virtual DbSet<ProductKeyword> ProductKeywords { get; set; }
        public virtual DbSet<ProductMedia> ProductMedia { get; set; }
        public virtual DbSet<ProductOrder> ProductOrders { get; set; }
        public virtual DbSet<ProductPricePoint> ProductPricePoints { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }


        public virtual DbSet<Subgroup> Subgroups { get; set; }
        public virtual DbSet<SubgroupProduct> SubgroupProducts { get; set; }

        public virtual DbSet<Vendor> Vendors { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");



            // Categories
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(x => x.Media)
                .WithMany(x => x.Categtories)
                .OnDelete(DeleteBehavior.Restrict);
            });




            // Customers
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable(name: "Customers");
                entity.Property(x => x.EmailPrefNameChange).HasDefaultValue(true);
                entity.Property(x => x.EmailPrefEmailChange).HasDefaultValue(true);
                entity.Property(x => x.EmailPrefPasswordChange).HasDefaultValue(true);
                entity.Property(x => x.EmailPrefProfilePicChange).HasDefaultValue(true);
                entity.Property(x => x.EmailPrefNewCollaborator).HasDefaultValue(true);
                entity.Property(x => x.EmailPrefRemovedCollaborator).HasDefaultValue(true);
                entity.Property(x => x.EmailPrefRemovedListItem).HasDefaultValue(true);
                entity.Property(x => x.EmailPrefMovedListItem).HasDefaultValue(true);
                entity.Property(x => x.EmailPrefAddedListItem).HasDefaultValue(true);
                entity.Property(x => x.EmailPrefListNameChange).HasDefaultValue(true);
                entity.Property(x => x.EmailPrefDeletedList).HasDefaultValue(true);
                entity.Property(x => x.EmailPrefReview).HasDefaultValue(true);
            });




            // Keywords
            modelBuilder.Entity<Keyword>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });


            // KeywordSearchVolumes
            modelBuilder.Entity<KeywordSearchVolume>(entity =>
            {
                entity.HasKey(e => new { e.KeywordId, e.Date })
                    .HasName("PK_KeywordSearchVolumes");
            });




            // ListProducts
            modelBuilder.Entity<ListProduct>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.CollaboratorId })
                    .HasName("PK_ListProducts");
            });



            // NotificationText
            modelBuilder.Entity<NotificationText>(entity =>
            {
                entity.HasOne(x => x.Customer)
                .WithMany(x => x.NotificationText)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Notification)
                .WithMany(x => x.NotificationText)
                .OnDelete(DeleteBehavior.Cascade);
            });




            




            // Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(x => x.Media)
                .WithMany(x => x.Products)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(x => x.Vendor)
                .WithMany(x => x.Products)
                .OnDelete(DeleteBehavior.Cascade);
            });




            // ProductFilters
            modelBuilder.Entity<ProductFilter>(entity =>
            {
                entity.HasKey(x => new { x.ProductId, x.FilterOptionId })
                .HasName("PK_ProductFilters");
            });






            // ProductContent
            modelBuilder.Entity<ProductContent>(entity =>
            {
                entity.HasOne(x => x.Media)
                .WithMany(x => x.ProductContent)
                .OnDelete(DeleteBehavior.Restrict);
            });




            // ProductMedia
            modelBuilder.Entity<ProductMedia>(entity =>
            {
                entity.HasOne(x => x.Media)
                .WithMany(x => x.ProductMedia)
                .OnDelete(DeleteBehavior.Restrict);
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
