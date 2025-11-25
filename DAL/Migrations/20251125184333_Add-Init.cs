using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameStateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CardIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFaceUp = table.Column<bool>(type: "INTEGER", nullable: false),
                    Column = table.Column<int>(type: "INTEGER", nullable: false),
                    Row = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Player1Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Player2Id = table.Column<Guid>(type: "TEXT", nullable: true),
                    CurrentTurnId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CurrentFlippedCardId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Player1Score = table.Column<int>(type: "INTEGER", nullable: false),
                    Player2Score = table.Column<int>(type: "INTEGER", nullable: false),
                    IsGameOver = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSinglePlayer = table.Column<bool>(type: "INTEGER", nullable: false),
                    GameStartTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameStates_CardStates_CurrentFlippedCardId",
                        column: x => x.CurrentFlippedCardId,
                        principalTable: "CardStates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameStates_Players_CurrentTurnId",
                        column: x => x.CurrentTurnId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameStates_Players_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameStates_Players_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "Players",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Time = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    GameStateId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scores_GameStates_GameStateId",
                        column: x => x.GameStateId,
                        principalTable: "GameStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardStates_GameStateId",
                table: "CardStates",
                column: "GameStateId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStates_CurrentFlippedCardId",
                table: "GameStates",
                column: "CurrentFlippedCardId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStates_CurrentTurnId",
                table: "GameStates",
                column: "CurrentTurnId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStates_Player1Id",
                table: "GameStates",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_GameStates_Player2Id",
                table: "GameStates",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_GameStateId",
                table: "Scores",
                column: "GameStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardStates_GameStates_GameStateId",
                table: "CardStates",
                column: "GameStateId",
                principalTable: "GameStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardStates_GameStates_GameStateId",
                table: "CardStates");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "GameStates");

            migrationBuilder.DropTable(
                name: "CardStates");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
