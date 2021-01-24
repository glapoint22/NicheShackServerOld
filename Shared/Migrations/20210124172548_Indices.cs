using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class Indices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubgroupProducts_SubgroupId",
                table: "SubgroupProducts");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_ProductId",
                table: "ProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_ProductPricePoints_ProductId",
                table: "ProductPricePoints");

            migrationBuilder.DropIndex(
                name: "IX_ProductOrders_CustomerId",
                table: "ProductOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProductMedia_ProductId",
                table: "ProductMedia");

            migrationBuilder.DropIndex(
                name: "IX_ProductKeywords_KeywordId",
                table: "ProductKeywords");

            migrationBuilder.DropIndex(
                name: "IX_ProductContent_ProductId",
                table: "ProductContent");

            migrationBuilder.DropIndex(
                name: "IX_PriceIndices_ProductContentId",
                table: "PriceIndices");

            migrationBuilder.DropIndex(
                name: "IX_OrderProducts_OrderId",
                table: "OrderProducts");

            migrationBuilder.DropIndex(
                name: "IX_Niches_CategoryId",
                table: "Niches");

            migrationBuilder.DropIndex(
                name: "IX_ListProducts_CollaboratorId",
                table: "ListProducts");

            migrationBuilder.DropIndex(
                name: "IX_ListCollaborators_CustomerId",
                table: "ListCollaborators");

            migrationBuilder.DropIndex(
                name: "IX_ListCollaborators_ListId",
                table: "ListCollaborators");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_SubgroupProducts_SubgroupId",
                table: "SubgroupProducts",
                column: "SubgroupId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_UrlId",
                table: "Products",
                column: "UrlId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "ImageId", "Name", "Hoplink", "Description", "MinPrice", "MaxPrice", "TotalReviews", "Rating", "OneStar", "TwoStars", "ThreeStars", "FourStars", "FiveStars", "UrlName" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name_Id",
                table: "Products",
                columns: new[] { "Name", "Id" })
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "ImageId", "NicheId", "UrlId", "UrlName", "MinPrice", "MaxPrice", "TotalReviews", "Rating", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductId_Deleted",
                table: "ProductReviews",
                columns: new[] { "ProductId", "Deleted" })
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "CustomerId", "Title", "Rating", "Date", "Text", "Likes", "Dislikes", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricePoints_ProductId",
                table: "ProductPricePoints",
                column: "ProductId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "TextBefore", "WholeNumber", "Decimal", "TextAfter", "Index" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrders_CustomerId",
                table: "ProductOrders",
                column: "CustomerId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "ProductId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrders_Date",
                table: "ProductOrders",
                column: "Date")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrders_CustomerId_Date",
                table: "ProductOrders",
                columns: new[] { "CustomerId", "Date" })
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "ProductId", "PaymentMethod", "Subtotal", "ShippingHandling", "Discount", "Tax", "Total" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedia_ProductId",
                table: "ProductMedia",
                column: "ProductId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "MediaId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductKeywords_KeywordId",
                table: "ProductKeywords",
                column: "KeywordId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductContent_ProductId",
                table: "ProductContent",
                column: "ProductId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "IconId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_PriceIndices_ProductContentId",
                table: "PriceIndices",
                column: "ProductContentId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "Index" });

            migrationBuilder.CreateIndex(
                name: "IX_Pages_DisplayType",
                table: "Pages",
                column: "DisplayType")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Content" });

            migrationBuilder.CreateIndex(
                name: "IX_Pages_UrlId",
                table: "Pages",
                column: "UrlId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Content" });

            migrationBuilder.CreateIndex(
                name: "IX_PageReferenceItems_ItemId",
                table: "PageReferenceItems",
                column: "ItemId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "PageId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_OrderId",
                table: "OrderProducts",
                column: "OrderId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "Name", "Quantity", "Price", "LineItemType", "RebillFrequency", "RebillAmount", "PaymentsRemaining" });

            migrationBuilder.CreateIndex(
                name: "IX_Niches_CategoryId",
                table: "Niches",
                column: "CategoryId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "UrlId", "UrlName", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Niches_UrlId",
                table: "Niches",
                column: "UrlId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Lists_CollaborateId",
                table: "Lists",
                column: "CollaborateId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "Name", "Description" });

            migrationBuilder.CreateIndex(
                name: "IX_ListProducts_CollaboratorId",
                table: "ListProducts",
                column: "CollaboratorId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "ProductId", "DateAdded" });

            migrationBuilder.CreateIndex(
                name: "IX_ListCollaborators_ListId_IsOwner",
                table: "ListCollaborators",
                columns: new[] { "ListId", "IsOwner" })
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_ListCollaborators_ListId_IsRemoved",
                table: "ListCollaborators",
                columns: new[] { "ListId", "IsRemoved" })
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "CustomerId", "IsOwner" });

            migrationBuilder.CreateIndex(
                name: "IX_ListCollaborators_CustomerId_ListId_IsRemoved",
                table: "ListCollaborators",
                columns: new[] { "CustomerId", "ListId", "IsRemoved" })
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "IsOwner" });

            migrationBuilder.CreateIndex(
                name: "IX_KeywordSearchVolumes_Date",
                table: "KeywordSearchVolumes",
                column: "Date")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "KeywordId" });

            migrationBuilder.CreateIndex(
                name: "IX_Filters_Name",
                table: "Filters",
                column: "Name")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_FilterOptions_Id_FilterId",
                table: "FilterOptions",
                columns: new[] { "Id", "FilterId" })
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Emails_Name",
                table: "Emails",
                column: "Name")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Content" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefAddedListItem",
                table: "Customers",
                column: "EmailPrefAddedListItem")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefDeletedList",
                table: "Customers",
                column: "EmailPrefDeletedList")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefEmailChange",
                table: "Customers",
                column: "EmailPrefEmailChange")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefListNameChange",
                table: "Customers",
                column: "EmailPrefListNameChange")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefMovedListItem",
                table: "Customers",
                column: "EmailPrefMovedListItem")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefNameChange",
                table: "Customers",
                column: "EmailPrefNameChange")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefNewCollaborator",
                table: "Customers",
                column: "EmailPrefNewCollaborator")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefPasswordChange",
                table: "Customers",
                column: "EmailPrefPasswordChange")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefProfilePicChange",
                table: "Customers",
                column: "EmailPrefProfilePicChange")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefRemovedCollaborator",
                table: "Customers",
                column: "EmailPrefRemovedCollaborator")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefRemovedListItem",
                table: "Customers",
                column: "EmailPrefRemovedListItem")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailPrefReview",
                table: "Customers",
                column: "EmailPrefReview")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Id",
                table: "Customers",
                column: "Id")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "FirstName", "Image" });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Customers",
                column: "NormalizedEmail")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "UserName", "NormalizedUserName", "Email", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "FirstName", "LastName", "ReviewName", "Image", "EmailPrefAddedListItem", "EmailPrefDeletedList", "EmailPrefEmailChange", "EmailPrefListNameChange", "EmailPrefMovedListItem", "EmailPrefNameChange", "EmailPrefNewCollaborator", "EmailPrefPasswordChange", "EmailPrefProfilePicChange", "EmailPrefRemovedCollaborator", "EmailPrefRemovedListItem", "EmailPrefReview" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubgroupProducts_SubgroupId",
                table: "SubgroupProducts");

            migrationBuilder.DropIndex(
                name: "IX_Products_UrlId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Name_Id",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_ProductId_Deleted",
                table: "ProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_ProductPricePoints_ProductId",
                table: "ProductPricePoints");

            migrationBuilder.DropIndex(
                name: "IX_ProductOrders_CustomerId",
                table: "ProductOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProductOrders_Date",
                table: "ProductOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProductOrders_CustomerId_Date",
                table: "ProductOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProductMedia_ProductId",
                table: "ProductMedia");

            migrationBuilder.DropIndex(
                name: "IX_ProductKeywords_KeywordId",
                table: "ProductKeywords");

            migrationBuilder.DropIndex(
                name: "IX_ProductContent_ProductId",
                table: "ProductContent");

            migrationBuilder.DropIndex(
                name: "IX_PriceIndices_ProductContentId",
                table: "PriceIndices");

            migrationBuilder.DropIndex(
                name: "IX_Pages_DisplayType",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_UrlId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_PageReferenceItems_ItemId",
                table: "PageReferenceItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderProducts_OrderId",
                table: "OrderProducts");

            migrationBuilder.DropIndex(
                name: "IX_Niches_CategoryId",
                table: "Niches");

            migrationBuilder.DropIndex(
                name: "IX_Niches_UrlId",
                table: "Niches");

            migrationBuilder.DropIndex(
                name: "IX_Lists_CollaborateId",
                table: "Lists");

            migrationBuilder.DropIndex(
                name: "IX_ListProducts_CollaboratorId",
                table: "ListProducts");

            migrationBuilder.DropIndex(
                name: "IX_ListCollaborators_ListId_IsOwner",
                table: "ListCollaborators");

            migrationBuilder.DropIndex(
                name: "IX_ListCollaborators_ListId_IsRemoved",
                table: "ListCollaborators");

            migrationBuilder.DropIndex(
                name: "IX_ListCollaborators_CustomerId_ListId_IsRemoved",
                table: "ListCollaborators");

            migrationBuilder.DropIndex(
                name: "IX_KeywordSearchVolumes_Date",
                table: "KeywordSearchVolumes");

            migrationBuilder.DropIndex(
                name: "IX_Filters_Name",
                table: "Filters");

            migrationBuilder.DropIndex(
                name: "IX_FilterOptions_Id_FilterId",
                table: "FilterOptions");

            migrationBuilder.DropIndex(
                name: "IX_Emails_Name",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefAddedListItem",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefDeletedList",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefEmailChange",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefListNameChange",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefMovedListItem",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefNameChange",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefNewCollaborator",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefPasswordChange",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefProfilePicChange",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefRemovedCollaborator",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefRemovedListItem",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailPrefReview",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Id",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_SubgroupProducts_SubgroupId",
                table: "SubgroupProducts",
                column: "SubgroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductId",
                table: "ProductReviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricePoints_ProductId",
                table: "ProductPricePoints",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrders_CustomerId",
                table: "ProductOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedia_ProductId",
                table: "ProductMedia",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductKeywords_KeywordId",
                table: "ProductKeywords",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductContent_ProductId",
                table: "ProductContent",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceIndices_ProductContentId",
                table: "PriceIndices",
                column: "ProductContentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_OrderId",
                table: "OrderProducts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Niches_CategoryId",
                table: "Niches",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ListProducts_CollaboratorId",
                table: "ListProducts",
                column: "CollaboratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ListCollaborators_CustomerId",
                table: "ListCollaborators",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ListCollaborators_ListId",
                table: "ListCollaborators",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Customers",
                column: "NormalizedEmail");
        }
    }
}
