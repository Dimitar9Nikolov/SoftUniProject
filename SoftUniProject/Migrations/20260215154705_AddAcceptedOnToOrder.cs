using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftUniProject.Migrations
{
    /// <inheritdoc />
    public partial class AddAcceptedOnToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedOn",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedOn",
                table: "Orders");
        }
    }
}
