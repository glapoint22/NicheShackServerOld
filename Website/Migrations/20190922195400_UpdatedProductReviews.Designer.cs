﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using DataAccess.Models;

namespace Website.Migrations
{
    [DbContext(typeof(NicheShackContext))]
    [Migration("20190922195400_UpdatedProductReviews")]
    partial class UpdatedProductReviews
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<string>", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Website.Models.Category", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Website.Models.Customer", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .IsUnicode(false);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .IsUnicode(false);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("ReviewName")
                        .IsRequired()
                        .IsUnicode(false);

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Website.Models.Filter", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Filters");
                });

            modelBuilder.Entity("Website.Models.FilterOption", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("FilterId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("FilterId");

                    b.ToTable("FilterOptions");
                });

            modelBuilder.Entity("Website.Models.List", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .IsUnicode(false);

                    b.Property<string>("CollaborateId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(false);

                    b.Property<string>("Description")
                        .HasColumnType("varchar(max)")
                        .IsUnicode(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Lists");
                });

            modelBuilder.Entity("Website.Models.ListCollaborator", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<bool>("IsOwner");

                    b.Property<string>("ListId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ListId");

                    b.ToTable("ListCollaborators");
                });

            modelBuilder.Entity("Website.Models.ListProduct", b =>
                {
                    b.Property<string>("ProductId")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<Guid>("CollaboratorId")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime");

                    b.HasKey("ProductId", "CollaboratorId")
                        .HasName("PK_ListProducts");

                    b.HasIndex("CollaboratorId");

                    b.ToTable("ListProducts");
                });

            modelBuilder.Entity("Website.Models.Niche", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("CategoryId");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Niches");
                });

            modelBuilder.Entity("Website.Models.OrderProduct", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(25)
                        .IsUnicode(false);

                    b.Property<string>("OrderId");

                    b.Property<bool>("IsMain");

                    b.Property<double>("Price");

                    b.Property<int>("Quantity");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<int>("Type");

                    b.HasKey("Id", "OrderId")
                        .HasName("PK_OrderProducts");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderProducts");
                });

            modelBuilder.Entity("Website.Models.PriceRange", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Label")
                        .IsRequired();

                    b.Property<int>("Max");

                    b.Property<int>("Min");

                    b.HasKey("Id");

                    b.ToTable("PriceRanges");
                });

            modelBuilder.Entity("Website.Models.Product", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.Property<bool>("Featured");

                    b.Property<double>("FiveStars");

                    b.Property<double>("FourStars");

                    b.Property<string>("Hoplink")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<double>("MaxPrice");

                    b.Property<double>("MinPrice");

                    b.Property<int>("NicheId");

                    b.Property<double>("OneStar");

                    b.Property<double>("Rating");

                    b.Property<string>("ShareImage")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<double>("ThreeStars");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<int>("TotalReviews");

                    b.Property<double>("TwoStars");

                    b.Property<string>("UrlTitle")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("NicheId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Website.Models.ProductContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("PriceIndices")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductContent");
                });

            modelBuilder.Entity("Website.Models.ProductFilter", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("FilterOptionId");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("FilterOptionId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductFilters");
                });

            modelBuilder.Entity("Website.Models.ProductMedia", b =>
                {
                    b.Property<string>("ProductId")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<string>("Url")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("Thumbnail")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<int>("Type");

                    b.HasKey("ProductId", "Url")
                        .HasName("PK_ProductMedia");

                    b.ToTable("ProductMedia");
                });

            modelBuilder.Entity("Website.Models.ProductOrder", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(21)
                        .IsUnicode(false);

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<double>("Discount");

                    b.Property<int>("PaymentMethod");

                    b.Property<string>("ProductId")
                        .IsRequired();

                    b.Property<double>("ShippingHandling");

                    b.Property<double>("Subtotal");

                    b.Property<double>("Tax");

                    b.Property<double>("Total");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductOrders");
                });

            modelBuilder.Entity("Website.Models.ProductPricePoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Frequency");

                    b.Property<double>("Price");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductPricePoints");
                });

            modelBuilder.Entity("Website.Models.ProductReview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<DateTime>("Date")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    b.Property<int>("Dislikes");

                    b.Property<bool>("IsVerified");

                    b.Property<int>("Likes");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<double>("Rating");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("varchar(max)")
                        .IsUnicode(false);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductReviews");
                });

            modelBuilder.Entity("Website.Models.RefreshToken", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("CustomerId")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("datetime");

                    b.HasKey("Id", "CustomerId")
                        .HasName("PK_RefreshTokens");

                    b.HasIndex("CustomerId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<string>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Website.Models.Customer")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Website.Models.Customer")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<string>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Website.Models.Customer")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Website.Models.Customer")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.FilterOption", b =>
                {
                    b.HasOne("Website.Models.Filter", "Filter")
                        .WithMany("FilterOptions")
                        .HasForeignKey("FilterId")
                        .HasConstraintName("FK_FilterOptions_Filters")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.ListCollaborator", b =>
                {
                    b.HasOne("Website.Models.Customer", "Customer")
                        .WithMany("ListCollaborators")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("FK_ListCollaborators_Customers")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Website.Models.List", "List")
                        .WithMany("Collaborators")
                        .HasForeignKey("ListId")
                        .HasConstraintName("FK_ListCollaborators_Lists")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.ListProduct", b =>
                {
                    b.HasOne("Website.Models.ListCollaborator", "Collaborator")
                        .WithMany("ListProducts")
                        .HasForeignKey("CollaboratorId")
                        .HasConstraintName("FK_ListProducts_ListCollaborators")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Website.Models.Product", "Product")
                        .WithMany("ListProducts")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ListProducts_Products")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.Niche", b =>
                {
                    b.HasOne("Website.Models.Category", "Category")
                        .WithMany("Niches")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_Niches_Categories")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.OrderProduct", b =>
                {
                    b.HasOne("Website.Models.ProductOrder", "ProductOrder")
                        .WithMany("OrderProducts")
                        .HasForeignKey("OrderId")
                        .HasConstraintName("FK_OrderProducts_ProductOrders")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.Product", b =>
                {
                    b.HasOne("Website.Models.Niche", "Niche")
                        .WithMany("Products")
                        .HasForeignKey("NicheId")
                        .HasConstraintName("FK_Products_Niches")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.ProductContent", b =>
                {
                    b.HasOne("Website.Models.Product", "Product")
                        .WithMany("ProductContent")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductContent_Products")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.ProductFilter", b =>
                {
                    b.HasOne("Website.Models.FilterOption", "FilterOption")
                        .WithMany("ProductFilters")
                        .HasForeignKey("FilterOptionId")
                        .HasConstraintName("FK_ProductFilters_FilterOptions")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Website.Models.Product", "Product")
                        .WithMany("ProductFilters")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductFilters_Products")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.ProductMedia", b =>
                {
                    b.HasOne("Website.Models.Product", "Product")
                        .WithMany("ProductMedia")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductMedia_Products")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.ProductOrder", b =>
                {
                    b.HasOne("Website.Models.Customer", "Customer")
                        .WithMany("ProductOrders")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("FK_ProductOrders_Customers")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Website.Models.Product", "Product")
                        .WithMany("ProductOrders")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductOrders_Products")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.ProductPricePoint", b =>
                {
                    b.HasOne("Website.Models.Product", "Product")
                        .WithMany("ProductPricePoints")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductPricePoints_Products")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.ProductReview", b =>
                {
                    b.HasOne("Website.Models.Customer", "Customer")
                        .WithMany("ProductReviews")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("FK_ProductReviews_Customers")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Website.Models.Product", "Product")
                        .WithMany("ProductReviews")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductReviews_Products")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Website.Models.RefreshToken", b =>
                {
                    b.HasOne("Website.Models.Customer", "Customer")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("FK_RefreshTokens_Customers")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
