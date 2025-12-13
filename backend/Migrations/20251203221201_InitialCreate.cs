using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MusicianProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LookingFor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Instruments = table.Column<string[]>(type: "text[]", nullable: false),
                    Genres = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicianProfiles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusicianProfiles");
        }
    }
}
