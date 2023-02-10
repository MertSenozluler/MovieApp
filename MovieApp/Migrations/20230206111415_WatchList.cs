using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApp.Migrations
{
    /// <inheritdoc />
    public partial class WatchList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Movie",
                table: "UserBalanceMovie",
                newName: "Role");

            migrationBuilder.AddColumn<string>(
                name: "BuyMovie",
                table: "UserBalanceMovie",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "UserBalanceMovie",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AdminBalance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Balance = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminBalance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WatchList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MovieName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WatchType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchList", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminBalance");

            migrationBuilder.DropTable(
                name: "WatchList");

            migrationBuilder.DropColumn(
                name: "BuyMovie",
                table: "UserBalanceMovie");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "UserBalanceMovie");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "UserBalanceMovie",
                newName: "Movie");
        }
    }
}
