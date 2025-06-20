using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JokenpoApiRest.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HandRelations",
                columns: table => new
                {
                    WinnerHandId = table.Column<int>(type: "integer", nullable: false),
                    LoserHandId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandRelations", x => new { x.WinnerHandId, x.LoserHandId });
                    table.ForeignKey(
                        name: "FK_HandRelations_Hands_LoserHandId",
                        column: x => x.LoserHandId,
                        principalTable: "Hands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HandRelations_Hands_WinnerHandId",
                        column: x => x.WinnerHandId,
                        principalTable: "Hands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participations",
                columns: table => new
                {
                    RoundId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    HandId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participations", x => new { x.UserId, x.RoundId });
                    table.ForeignKey(
                        name: "FK_Participations_Hands_HandId",
                        column: x => x.HandId,
                        principalTable: "Hands",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Participations_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Hands",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Pedra" },
                    { 2, "Papel" },
                    { 3, "Tesoura" },
                    { 4, "Spock" },
                    { 5, "Lagarto" }
                });

            migrationBuilder.InsertData(
                table: "HandRelations",
                columns: new[] { "LoserHandId", "WinnerHandId" },
                values: new object[,]
                {
                    { 3, 1 },
                    { 5, 1 },
                    { 1, 2 },
                    { 4, 2 },
                    { 2, 3 },
                    { 5, 3 },
                    { 1, 4 },
                    { 3, 4 },
                    { 2, 5 },
                    { 4, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HandRelations_LoserHandId",
                table: "HandRelations",
                column: "LoserHandId");

            migrationBuilder.CreateIndex(
                name: "IX_Hands_Name",
                table: "Hands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participations_HandId",
                table: "Participations",
                column: "HandId");

            migrationBuilder.CreateIndex(
                name: "IX_Participations_RoundId",
                table: "Participations",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HandRelations");

            migrationBuilder.DropTable(
                name: "Participations");

            migrationBuilder.DropTable(
                name: "Hands");

            migrationBuilder.DropTable(
                name: "Rounds");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
