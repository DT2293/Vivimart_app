using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class DropInvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa ràng buộc khóa ngoại từ InvoiceDetails đến Invoices
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceId", // Tên ràng buộc khóa ngoại
                table: "InvoiceDetails");

            // Xóa bảng InvoiceDetails
            migrationBuilder.DropTable(
                name: "InvoiceDetails");

            // Xóa ràng buộc khóa ngoại từ Invoices đến User
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_User__UserId",
                table: "Invoices");

            // Xóa bảng Invoices
            migrationBuilder.DropTable(
                name: "Invoices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
