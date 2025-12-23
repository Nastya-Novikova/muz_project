using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LocalizedName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollaborationGoals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LocalizedName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollaborationGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailVerificationCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerificationCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LocalizedName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MusicalSpecializations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LocalizedName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicalSpecializations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Avatar = table.Column<byte[]>(type: "bytea", nullable: true),
                    ProfileCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FavoriteProfileIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollaborationSuggestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollaborationSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollaborationSuggestions_Users_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollaborationSuggestions_Users_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MusicianProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Avatar = table.Column<byte[]>(type: "bytea", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Telegram = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CityId = table.Column<int>(type: "integer", nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicianProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusicianProfiles_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MusicianProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioAudio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FileData = table.Column<byte[]>(type: "bytea", nullable: false),
                    MimeType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioAudio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioAudio_MusicianProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "MusicianProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioVideo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FileData = table.Column<byte[]>(type: "bytea", nullable: false),
                    MimeType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioVideo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioVideo_MusicianProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "MusicianProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileCollaborationGoal",
                columns: table => new
                {
                    CollaborationGoalsId = table.Column<int>(type: "integer", nullable: false),
                    ProfilesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileCollaborationGoal", x => new { x.CollaborationGoalsId, x.ProfilesId });
                    table.ForeignKey(
                        name: "FK_ProfileCollaborationGoal_CollaborationGoals_CollaborationGo~",
                        column: x => x.CollaborationGoalsId,
                        principalTable: "CollaborationGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileCollaborationGoal_MusicianProfiles_ProfilesId",
                        column: x => x.ProfilesId,
                        principalTable: "MusicianProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileGenre",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "integer", nullable: false),
                    ProfilesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileGenre", x => new { x.GenresId, x.ProfilesId });
                    table.ForeignKey(
                        name: "FK_ProfileGenre_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileGenre_MusicianProfiles_ProfilesId",
                        column: x => x.ProfilesId,
                        principalTable: "MusicianProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSpecialty",
                columns: table => new
                {
                    ProfilesId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecialtiesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSpecialty", x => new { x.ProfilesId, x.SpecialtiesId });
                    table.ForeignKey(
                        name: "FK_ProfileSpecialty_MusicalSpecializations_SpecialtiesId",
                        column: x => x.SpecialtiesId,
                        principalTable: "MusicalSpecializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileSpecialty_MusicianProfiles_ProfilesId",
                        column: x => x.ProfilesId,
                        principalTable: "MusicianProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "LocalizedName", "Name" },
                values: new object[,]
                {
                    { 1, "Москва", "Moscow" },
                    { 2, "Санкт-Петербург", "Saint Petersburg" },
                    { 3, "Новосибирск", "Novosibirsk" },
                    { 4, "Екатеринбург", "Yekaterinburg" },
                    { 5, "Казань", "Kazan" }
                });

            migrationBuilder.InsertData(
                table: "CollaborationGoals",
                columns: new[] { "Id", "LocalizedName", "Name" },
                values: new object[,]
                {
                    { 1, "Ищу участников в группу", "band" },
                    { 2, "Готов(а) к сессионной работе", "session" },
                    { 3, "Открыт(а) к совместным проектам", "collaboration" },
                    { 4, "Ищу продюсера", "producer" },
                    { 5, "Ищу исполнителя для песен", "artist" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "LocalizedName", "Name" },
                values: new object[,]
                {
                    { 1, "Джаз", "jazz" },
                    { 2, "Рок", "rock" },
                    { 3, "Классика", "classical" },
                    { 4, "Электроника", "electronic" },
                    { 5, "Поп", "pop" },
                    { 6, "Хип-хоп", "hip-hop" },
                    { 7, "Метал", "metal" },
                    { 8, "Блюз", "blues" }
                });

            migrationBuilder.InsertData(
                table: "MusicalSpecializations",
                columns: new[] { "Id", "LocalizedName", "Name" },
                values: new object[,]
                {
                    { 1, "Вокалист", "vocalist" },
                    { 2, "Гитарист", "guitarist" },
                    { 3, "Бас-гитарист", "bassist" },
                    { 4, "Ударник", "drummer" },
                    { 5, "Клавишник", "keyboardist" },
                    { 6, "Композитор", "composer" },
                    { 7, "Продюсер", "producer" },
                    { 8, "Звукорежиссёр", "sound-engineer" },
                    { 9, "Диджей", "dj" },
                    { 10, "Скрипач", "violinist" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollaborationSuggestions_FromUserId",
                table: "CollaborationSuggestions",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CollaborationSuggestions_ToUserId",
                table: "CollaborationSuggestions",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicianProfiles_CityId",
                table: "MusicianProfiles",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicianProfiles_UserId",
                table: "MusicianProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioAudio_ProfileId",
                table: "PortfolioAudio",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioVideo_ProfileId",
                table: "PortfolioVideo",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileCollaborationGoal_ProfilesId",
                table: "ProfileCollaborationGoal",
                column: "ProfilesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileGenre_ProfilesId",
                table: "ProfileGenre",
                column: "ProfilesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSpecialty_SpecialtiesId",
                table: "ProfileSpecialty",
                column: "SpecialtiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollaborationSuggestions");

            migrationBuilder.DropTable(
                name: "EmailVerificationCodes");

            migrationBuilder.DropTable(
                name: "PortfolioAudio");

            migrationBuilder.DropTable(
                name: "PortfolioVideo");

            migrationBuilder.DropTable(
                name: "ProfileCollaborationGoal");

            migrationBuilder.DropTable(
                name: "ProfileGenre");

            migrationBuilder.DropTable(
                name: "ProfileSpecialty");

            migrationBuilder.DropTable(
                name: "CollaborationGoals");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "MusicalSpecializations");

            migrationBuilder.DropTable(
                name: "MusicianProfiles");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
