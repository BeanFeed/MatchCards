using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCardPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Column",
                table: "CardStates");

            migrationBuilder.RenameColumn(
                name: "Row",
                table: "CardStates",
                newName: "Position");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Position",
                table: "CardStates",
                newName: "Row");

            migrationBuilder.AddColumn<int>(
                name: "Column",
                table: "CardStates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
