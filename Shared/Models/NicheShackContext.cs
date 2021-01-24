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


        public virtual DbSet<PageReferenceItem> PageReferenceItems { get; set; }


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
                entity
                .HasIndex(x => x.Id)
                .IncludeProperties(x => new
                {
                    x.FirstName,
                    x.Image
                })
                .IsClustered(false);

                entity
                .HasIndex(x => x.EmailPrefDeletedList)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);

                entity
                .HasIndex(x => x.EmailPrefRemovedListItem)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);

                entity
                .HasIndex(x => x.EmailPrefNewCollaborator)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);

                entity
                .HasIndex(x => x.EmailPrefAddedListItem)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);

                entity
                .HasIndex(x => x.EmailPrefListNameChange)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);

                entity
                .HasIndex(x => x.EmailPrefMovedListItem)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);

                entity
                .HasIndex(x => x.EmailPrefNameChange)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);

                entity
                .HasIndex(x => x.EmailPrefEmailChange)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);

                entity
                .HasIndex(x => x.EmailPrefPasswordChange)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);

                entity
                .HasIndex(x => x.EmailPrefProfilePicChange)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);

                entity
                .HasIndex(x => x.EmailPrefRemovedCollaborator)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);


                entity
                .HasIndex(x => x.EmailPrefReview)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);


                entity
                .HasIndex(x => x.NormalizedEmail)
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.UserName,
                    x.NormalizedUserName,
                    x.Email,
                    x.EmailConfirmed,
                    x.PasswordHash,
                    x.SecurityStamp,
                    x.ConcurrencyStamp,
                    x.PhoneNumber,
                    x.PhoneNumberConfirmed,
                    x.TwoFactorEnabled,
                    x.LockoutEnd,
                    x.LockoutEnabled,
                    x.AccessFailedCount,
                    x.FirstName,
                    x.LastName,
                    x.ReviewName,
                    x.Image,
                    x.EmailPrefAddedListItem,
                    x.EmailPrefDeletedList,
                    x.EmailPrefEmailChange,
                    x.EmailPrefListNameChange,
                    x.EmailPrefMovedListItem,
                    x.EmailPrefNameChange,
                    x.EmailPrefNewCollaborator,
                    x.EmailPrefPasswordChange,
                    x.EmailPrefProfilePicChange,
                    x.EmailPrefRemovedCollaborator,
                    x.EmailPrefRemovedListItem,
                    x.EmailPrefReview
                })
                .IsClustered(false);
            });




            // Emails
            modelBuilder.Entity<Email>(entity =>
            {
                entity
                .HasIndex(x => x.Name)
                .IncludeProperties(x => x.Content)
                .IsClustered(false);
            });



            // Filters
            modelBuilder.Entity<Filter>(entity =>
            {
                entity
                .HasIndex(x => x.Name)
                .IncludeProperties(x => x.Id)
                .IsClustered(false);
            });





            // FilterOptions
            modelBuilder.Entity<FilterOption>(entity =>
            {
                entity
                .HasIndex(x => new {
                    x.Id,
                    x.FilterId
                })
                .IncludeProperties(x => x.Name)
                .IsClustered(false);
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

                entity
                .HasIndex(x => x.Date)
                .IncludeProperties(x => x.KeywordId)
                .IsClustered(false);
            });



            // Lists
            modelBuilder.Entity<List>(entity =>
            {
                entity
                .HasIndex(x => x.CollaborateId)
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description
                })
                .IsClustered(false);
            });



            // ListCollaborators
            modelBuilder.Entity<ListCollaborator>(entity =>
            {
                entity
                .HasIndex(x => new
                {
                    x.CustomerId,
                    x.ListId,
                    x.IsRemoved
                })
                .IncludeProperties(x => x.IsOwner)
                .IsClustered(false);

                entity
                .HasIndex(x => new
                {
                    x.ListId,
                    x.IsOwner
                })
                .IncludeProperties(x => x.CustomerId)
                .IsClustered(false);

                entity
                .HasIndex(x => new
                {
                    x.ListId,
                    x.IsRemoved
                })
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.CustomerId,
                    x.IsOwner
                })
                .IsClustered(false);
            });







            // ListProducts
            modelBuilder.Entity<ListProduct>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.CollaboratorId })
                    .HasName("PK_ListProducts");

                entity
                .HasIndex(x => x.CollaboratorId)
                .IncludeProperties(x => new
                {
                    x.ProductId,
                    x.DateAdded
                })
                .IsClustered(false);
            });




            // Niches
            modelBuilder.Entity<Niche>(entity =>
            {
                entity
                .HasIndex(x => x.CategoryId)
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.UrlId,
                    x.UrlName,
                    x.Name
                })
                .IsClustered(false);

                entity
                .HasIndex(x => x.UrlId)
                .IsClustered(false);
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




            // OrderProducts
            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity
                .HasIndex(x => x.OrderId)
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.Name,
                    x.Quantity,
                    x.Price,
                    x.LineItemType,
                    x.RebillFrequency,
                    x.RebillAmount,
                    x.PaymentsRemaining
                })
                .IsClustered(false);
            });




            // Pages
            modelBuilder.Entity<Page>(entity =>
            {
                entity
                .HasIndex(x => x.DisplayType)
                .IncludeProperties(x => x.Content)
                .IsClustered(false);

                entity
                .HasIndex(x => x.UrlId)
                .IncludeProperties(x => x.Content)
                .IsClustered(false);
            });






            // PageReferenceItems
            modelBuilder.Entity<PageReferenceItem>(entity =>
            {
                entity
                .HasIndex(x => x.ItemId)
                .IncludeProperties(x => x.PageId)
                .IsClustered(false);
            });




            // PriceIndices
            modelBuilder.Entity<PriceIndex>(entity =>
            {
                entity
                .HasIndex(x => x.ProductContentId)
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.Index
                })
                .IsClustered(false);
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


                entity
                .HasIndex(x => x.UrlId)
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.ImageId,
                    x.Name,
                    x.Hoplink,
                    x.Description,
                    x.MinPrice,
                    x.MaxPrice,
                    x.TotalReviews,
                    x.Rating,
                    x.OneStar,
                    x.TwoStars,
                    x.ThreeStars,
                    x.FourStars,
                    x.FiveStars,
                    x.UrlName
                })
                .IsClustered(false);


                entity
                .HasIndex(x => new
                {
                    x.Name,
                    x.Id
                })
                .IncludeProperties(x => new
                {
                    x.ImageId,
                    x.NicheId,
                    x.UrlId,
                    x.UrlName,
                    x.MinPrice,
                    x.MaxPrice,
                    x.TotalReviews,
                    x.Rating,
                    x.Date
                })
                .IsClustered(false);
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


                entity
                .HasIndex(x => x.ProductId)
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.IconId,
                    x.Name
                })
                .IsClustered(false);
            });




            // ProductKeywords
            modelBuilder.Entity<ProductKeyword>(entity =>
            {
                entity
                .HasIndex(x => x.KeywordId)
                .IncludeProperties(x => x.ProductId)
                .IsClustered(false);
            });




            // ProductMedia
            modelBuilder.Entity<ProductMedia>(entity =>
            {
                entity.HasOne(x => x.Media)
                .WithMany(x => x.ProductMedia)
                .OnDelete(DeleteBehavior.Restrict);


                entity
                .HasIndex(x => x.ProductId)
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.MediaId
                })
                .IsClustered(false);
            });





            // ProductOrders
            modelBuilder.Entity<ProductOrder>(entity =>
            {
                entity
                .HasIndex(x => x.CustomerId)
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.ProductId,
                    x.Date
                })
                .IsClustered(false);


                entity
                .HasIndex(x => new
                {
                    x.CustomerId,
                    x.Date
                })
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.ProductId,
                    x.PaymentMethod,
                    x.Subtotal,
                    x.ShippingHandling,
                    x.Discount,
                    x.Tax,
                    x.Total
                })
                .IsClustered(false);



                entity
                .HasIndex(x => x.Date)
                .IncludeProperties(x => x.ProductId)
                .IsClustered(false);
            });






            // ProductPricePoints
            modelBuilder.Entity<ProductPricePoint>(entity =>
            {
                entity
                .HasIndex(x => x.ProductId)
                .IncludeProperties(x => new
                {
                    x.Id,
                    x.TextBefore,
                    x.WholeNumber,
                    x.Decimal,
                    x.TextAfter,
                    x.Index
                })
                .IsClustered(false);
            });





            // ProductReviews
            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity
                .HasIndex(x => new
                {
                    x.ProductId,
                    x.Deleted
                })
                .IncludeProperties(x => new
                {
                    x.CustomerId,
                    x.Title,
                    x.Rating,
                    x.Date,
                    x.Text,
                    x.Likes,
                    x.Dislikes,
                    x.Id
                })
                .IsClustered(false);
            });




            // RefreshTokens
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.CustomerId })
                    .HasName("PK_RefreshTokens");
            });



            // SubgroupProducts
            modelBuilder.Entity<SubgroupProduct>(entity =>
            {
                entity
                .HasIndex(x => x.SubgroupId)
                .IncludeProperties(x => x.ProductId)
                .IsClustered(false);
            });
        }
    }
}
