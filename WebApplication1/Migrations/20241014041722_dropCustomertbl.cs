using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class dropCustomertbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Invoices_Customers_CustomerId",
            //    table: "Invoices");

            //migrationBuilder.DropTable(
            //    name: "Customers");

            //migrationBuilder.DropIndex(
            //    name: "IX_Invoices_CustomerId",
            //    table: "Invoices");

            //migrationBuilder.DropColumn(
            //    name: "CustomerId",
            //    table: "Invoices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        //    migrationBuilder.AddColumn<string>(
        //        name: "CustomerId",
        //        table: "Invoices",
        //        type: "nvarchar(450)",
        //        nullable: true);

        //    migrationBuilder.CreateTable(
        //        name: "Customers",
        //        columns: table => new
        //        {
        //            CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
        //            Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
        //            Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
        //            PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
        //        },
        //        constraints: table =>
        //        {
        //            table.PrimaryKey("PK_Customers", x => x.CustomerId);
        //        }); 

        //    migrationBuilder.CreateIndex(
        //        name: "IX_Invoices_CustomerId",
        //        table: "Invoices",
        //        column: "CustomerId");

        //    migrationBuilder.AddForeignKey(
        //        name: "FK_Invoices_Customers_CustomerId",
        //        table: "Invoices",
        //        column: "CustomerId",
        //        principalTable: "Customers",
        //        principalColumn: "CustomerId");
        }
    }
}
