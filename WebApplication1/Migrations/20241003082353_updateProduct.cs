    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    #nullable disable

    namespace WebApplication1.Migrations
    {
        /// <inheritdoc />
        public partial class updateProduct : Migration
        {
            /// <inheritdoc />
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.CreateTable(
                    name: "ProductCategories",
                    columns: table => new
                    {
                        CategoryId = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                        UserId = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_ProductCategories", x => x.CategoryId);
                    });

                migrationBuilder.CreateTable(
                    name: "Products",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                        ExpDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                        UserId = table.Column<int>(type: "int", nullable: false),
                        ProductCategoryCategoryId = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Products", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Products_ProductCategories_ProductCategoryCategoryId",
                            column: x => x.ProductCategoryCategoryId,
                            principalTable: "ProductCategories",
                            principalColumn: "CategoryId",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_Products_ProductCategoryCategoryId",
                    table: "Products",
                    column: "ProductCategoryCategoryId");
            }

            /// <inheritdoc />
            protected override void Down(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.DropTable(
                    name: "Products");

                migrationBuilder.DropTable(
                    name: "ProductCategories");
            }
        }
    }
