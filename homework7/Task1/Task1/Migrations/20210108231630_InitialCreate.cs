using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Task1.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Runs",
                columns: table => new
                {
                    LaunchedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runs", x => x.LaunchedAt);
                });

            migrationBuilder.CreateTable(
                name: "AssemblyFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    TestRunLaunchedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssemblyFile_Runs_TestRunLaunchedAt",
                        column: x => x.TestRunLaunchedAt,
                        principalTable: "Runs",
                        principalColumn: "LaunchedAt",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestInfoModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MethodName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedException = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualException = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsIgnored = table.Column<bool>(type: "bit", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    ReasonToIgnore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestRunLaunchedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestInfoModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestInfoModel_Runs_TestRunLaunchedAt",
                        column: x => x.TestRunLaunchedAt,
                        principalTable: "Runs",
                        principalColumn: "LaunchedAt",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyFile_TestRunLaunchedAt",
                table: "AssemblyFile",
                column: "TestRunLaunchedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TestInfoModel_TestRunLaunchedAt",
                table: "TestInfoModel",
                column: "TestRunLaunchedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssemblyFile");

            migrationBuilder.DropTable(
                name: "TestInfoModel");

            migrationBuilder.DropTable(
                name: "Runs");
        }
    }
}
