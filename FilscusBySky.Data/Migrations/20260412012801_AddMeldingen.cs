using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilscusBySky.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMeldingen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeldingDag",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MeldingTijd",
                table: "AspNetUsers",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateTable(
                name: "Meldingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bericht = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsGelezen = table.Column<bool>(type: "bit", nullable: false),
                    AangemaaktOp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meldingen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meldingen_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meldingen_UserId",
                table: "Meldingen",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Meldingen");

            migrationBuilder.DropColumn(
                name: "MeldingDag",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MeldingTijd",
                table: "AspNetUsers");
        }
    }
}
