using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                schema: "orders",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderDate",
                schema: "orders",
                table: "orders");
        }
    }
}
