using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MusicianProfiles",
                columns: new[] { "Id", "Bio", "City", "FullName", "Genres", "Instruments", "LookingFor" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Jazz vocalist looking for a band", "Moscow", "Alice Johnson", new[] { "jazz", "soul" }, new[] { "vocals" }, "band" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Guitarist, session musician", "Saint Petersburg", "Bob Smith", new[] { "rock", "blues" }, new[] { "guitar", "bass" }, "session" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Pianist and composer", "Novosibirsk", "Clara Davis", new[] { "classical", "electronic" }, new[] { "piano", "synthesizer" }, "collaboration" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MusicianProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "MusicianProfiles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "MusicianProfiles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));
        }
    }
}
