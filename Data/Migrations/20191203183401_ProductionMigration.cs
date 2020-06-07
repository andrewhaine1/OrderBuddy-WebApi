using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ord.WebApi.Data.Migrations
{
    public partial class ProductionMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CollectionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    OauthId = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: true),
                    MobileNumber = table.Column<string>(nullable: true),
                    DeviceToken = table.Column<string>(nullable: true),
                    CanPlaceOrders = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Suburb = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdUserHomeAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    OrdUserId = table.Column<int>(nullable: false),
                    StreetAddress = table.Column<string>(nullable: false),
                    Suburb = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: false),
                    Province = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    HouseNumber = table.Column<string>(nullable: true),
                    UnitNumber = table.Column<string>(nullable: true),
                    ComplexName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdUserHomeAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdUserHomeAddresses_OrdUsers_OrdUserId",
                        column: x => x.OrdUserId,
                        principalTable: "OrdUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdUserWorkAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    OrdUserId = table.Column<int>(nullable: false),
                    StreetAddress = table.Column<string>(nullable: false),
                    Suburb = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: false),
                    Province = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    OfficeParkName = table.Column<string>(nullable: true),
                    BuildingName = table.Column<string>(nullable: true),
                    FloorNumber = table.Column<string>(nullable: true),
                    DepartmentName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdUserWorkAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdUserWorkAddresses_OrdUsers_OrdUserId",
                        column: x => x.OrdUserId,
                        principalTable: "OrdUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    OrdUserId = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: true),
                    ShopNumber = table.Column<string>(nullable: true),
                    ComplexName = table.Column<string>(nullable: true),
                    ShoppingCenterName = table.Column<string>(nullable: true),
                    StreetAddress = table.Column<string>(nullable: false),
                    Suburb = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: false),
                    Province = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: false),
                    ServiceAreaId = table.Column<int>(nullable: false),
                    RestaurantImagePath = table.Column<string>(nullable: true),
                    IsHalal = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsSuspended = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Restaurants_OrdUsers_OrdUserId",
                        column: x => x.OrdUserId,
                        principalTable: "OrdUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Restaurants_ServiceAreas_ServiceAreaId",
                        column: x => x.ServiceAreaId,
                        principalTable: "ServiceAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RestaurantId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    OrdUserId = table.Column<int>(nullable: false),
                    RestaurantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrdUsers_OrdUserId",
                        column: x => x.OrdUserId,
                        principalTable: "OrdUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantCollectionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RestaurantId = table.Column<int>(nullable: false),
                    CollectionTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantCollectionTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantCollectionTypes_CollectionTypes_CollectionTypeId",
                        column: x => x.CollectionTypeId,
                        principalTable: "CollectionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestaurantCollectionTypes_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantPaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RestaurantId = table.Column<int>(nullable: false),
                    PaymentMethodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantPaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantPaymentMethods_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestaurantPaymentMethods_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    MenuId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuCategories_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    OrderId = table.Column<int>(nullable: false),
                    DateTimePlaced = table.Column<DateTime>(nullable: false),
                    DateTimeAccepted = table.Column<DateTime>(nullable: false),
                    DateTimeReady = table.Column<DateTime>(nullable: false),
                    DateTimeCompleted = table.Column<DateTime>(nullable: false),
                    DateTimeCancelled = table.Column<DateTime>(nullable: false),
                    OrderTotal = table.Column<decimal>(nullable: false),
                    OrderStatus = table.Column<int>(nullable: false),
                    RestaurantPaymentMethodId = table.Column<int>(nullable: false),
                    RestaurantCollectionTypeId = table.Column<int>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    IsCancelled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderInfo_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderInfo_RestaurantCollectionTypes_RestaurantCollectionTyp~",
                        column: x => x.RestaurantCollectionTypeId,
                        principalTable: "RestaurantCollectionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderInfo_RestaurantPaymentMethods_RestaurantPaymentMethodId",
                        column: x => x.RestaurantPaymentMethodId,
                        principalTable: "RestaurantPaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CategoryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "MenuCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    OrderId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    SubTotal = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_MenuItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectionTypes_Name",
                table: "CollectionTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_MenuId_Name",
                table: "MenuCategories",
                columns: new[] { "MenuId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CategoryId",
                table: "MenuItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_RestaurantId_Name",
                table: "Menus",
                columns: new[] { "RestaurantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfo_OrderId",
                table: "OrderInfo",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfo_RestaurantCollectionTypeId",
                table: "OrderInfo",
                column: "RestaurantCollectionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfo_RestaurantPaymentMethodId",
                table: "OrderInfo",
                column: "RestaurantPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ItemId",
                table: "OrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrdUserId",
                table: "Orders",
                column: "OrdUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_RestaurantId",
                table: "Orders",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdUserHomeAddresses_OrdUserId",
                table: "OrdUserHomeAddresses",
                column: "OrdUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdUserWorkAddresses_OrdUserId",
                table: "OrdUserWorkAddresses",
                column: "OrdUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCollectionTypes_CollectionTypeId",
                table: "RestaurantCollectionTypes",
                column: "CollectionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCollectionTypes_RestaurantId_CollectionTypeId",
                table: "RestaurantCollectionTypes",
                columns: new[] { "RestaurantId", "CollectionTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantPaymentMethods_PaymentMethodId",
                table: "RestaurantPaymentMethods",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantPaymentMethods_RestaurantId_PaymentMethodId",
                table: "RestaurantPaymentMethods",
                columns: new[] { "RestaurantId", "PaymentMethodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_OrdUserId",
                table: "Restaurants",
                column: "OrdUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_ServiceAreaId",
                table: "Restaurants",
                column: "ServiceAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderInfo");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OrdUserHomeAddresses");

            migrationBuilder.DropTable(
                name: "OrdUserWorkAddresses");

            migrationBuilder.DropTable(
                name: "RestaurantCollectionTypes");

            migrationBuilder.DropTable(
                name: "RestaurantPaymentMethods");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "CollectionTypes");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "MenuCategories");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "Restaurants");

            migrationBuilder.DropTable(
                name: "OrdUsers");

            migrationBuilder.DropTable(
                name: "ServiceAreas");
        }
    }
}
