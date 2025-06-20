using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuHackathon2025.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    EmployeeCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StorageGB = table.Column<decimal>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageUsages_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "EmployeeCount", "Name" },
                values: new object[,]
                {
                    { 1, 12, "Architecture" },
                    { 2, 8, "Administration" },
                    { 3, 15, "Project Management" },
                    { 4, 25, "Development" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_StorageUsages_DepartmentId",
                table: "StorageUsages",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StorageUsages");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
