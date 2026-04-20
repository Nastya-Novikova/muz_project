using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MusicianFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LocalizedName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollaborationGoal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LocalizedName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollaborationGoal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailVerificationCode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerificationCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LocalizedName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MusicalSpecialty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LocalizedName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicalSpecialty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocalizedName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ProfileCreated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Role = table.Column<string>(type: "text", nullable: false, defaultValue: "User"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MusicianProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileType = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Telegram = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    VkUserId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    NotifyByEmail = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    NotifyByVk = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CityId = table.Column<int>(type: "integer", nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LookingFor = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicianProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusicianProfile_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MusicianProfile_User_Id",
                        column: x => x.Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollaborationSuggestion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollaborationSuggestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollaborationSuggestion_MusicianProfile_FromProfileId",
                        column: x => x.FromProfileId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollaborationSuggestion_MusicianProfile_ToProfileId",
                        column: x => x.ToProfileId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    RegionId = table.Column<int>(type: "integer", nullable: false),
                    CityId = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Scheduled"),
                    CreatorProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Event_MusicianProfile_CreatorProfileId",
                        column: x => x.CreatorProfileId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Event_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Favorite",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorite", x => new { x.UserId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_Favorite_MusicianProfile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorite_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_MusicianProfile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "MusicianProfile",
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
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    MimeType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioAudio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioAudio_MusicianProfile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioPhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    MimeType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioPhoto_MusicianProfile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "MusicianProfile",
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
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    MimeType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioVideo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioVideo_MusicianProfile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileCollaborationGoals",
                columns: table => new
                {
                    CollaborationGoalsId = table.Column<int>(type: "integer", nullable: false),
                    ProfilesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileCollaborationGoals", x => new { x.CollaborationGoalsId, x.ProfilesId });
                    table.ForeignKey(
                        name: "FK_ProfileCollaborationGoals_CollaborationGoal_CollaborationGo~",
                        column: x => x.CollaborationGoalsId,
                        principalTable: "CollaborationGoal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileCollaborationGoals_MusicianProfile_ProfilesId",
                        column: x => x.ProfilesId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileDesiredGenres",
                columns: table => new
                {
                    DesiredGenresId = table.Column<int>(type: "integer", nullable: false),
                    ProfilesLookingForThisGenreId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileDesiredGenres", x => new { x.DesiredGenresId, x.ProfilesLookingForThisGenreId });
                    table.ForeignKey(
                        name: "FK_ProfileDesiredGenres_Genre_DesiredGenresId",
                        column: x => x.DesiredGenresId,
                        principalTable: "Genre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileDesiredGenres_MusicianProfile_ProfilesLookingForThis~",
                        column: x => x.ProfilesLookingForThisGenreId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileDesiredSpecialties",
                columns: table => new
                {
                    DesiredSpecialtiesId = table.Column<int>(type: "integer", nullable: false),
                    ProfilesLookingForThisSpecialtyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileDesiredSpecialties", x => new { x.DesiredSpecialtiesId, x.ProfilesLookingForThisSpecialtyId });
                    table.ForeignKey(
                        name: "FK_ProfileDesiredSpecialties_MusicalSpecialty_DesiredSpecialti~",
                        column: x => x.DesiredSpecialtiesId,
                        principalTable: "MusicalSpecialty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileDesiredSpecialties_MusicianProfile_ProfilesLookingFo~",
                        column: x => x.ProfilesLookingForThisSpecialtyId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileGenres",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "integer", nullable: false),
                    ProfilesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileGenres", x => new { x.GenresId, x.ProfilesId });
                    table.ForeignKey(
                        name: "FK_ProfileGenres_Genre_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileGenres_MusicianProfile_ProfilesId",
                        column: x => x.ProfilesId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSpecialties",
                columns: table => new
                {
                    ProfilesId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecialtiesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSpecialties", x => new { x.ProfilesId, x.SpecialtiesId });
                    table.ForeignKey(
                        name: "FK_ProfileSpecialties_MusicalSpecialty_SpecialtiesId",
                        column: x => x.SpecialtiesId,
                        principalTable: "MusicalSpecialty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileSpecialties_MusicianProfile_ProfilesId",
                        column: x => x.ProfilesId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventRegistration",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRegistration", x => new { x.EventId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_EventRegistration_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventRegistration_MusicianProfile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "MusicianProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollaborationSuggestion_FromProfileId",
                table: "CollaborationSuggestion",
                column: "FromProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CollaborationSuggestion_ToProfileId",
                table: "CollaborationSuggestion",
                column: "ToProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationCode_Email_Code_IsUsed",
                table: "EmailVerificationCode",
                columns: new[] { "Email", "Code", "IsUsed" });

            migrationBuilder.CreateIndex(
                name: "IX_Event_CityId",
                table: "Event",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CreatorProfileId",
                table: "Event",
                column: "CreatorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_RegionId",
                table: "Event",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistration_EventId_ProfileId",
                table: "EventRegistration",
                columns: new[] { "EventId", "ProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistration_ProfileId",
                table: "EventRegistration",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_ProfileId",
                table: "Favorite",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicianProfile_CityId",
                table: "MusicianProfile",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicianProfile_Email",
                table: "MusicianProfile",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ProfileId",
                table: "Notification",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioAudio_ProfileId",
                table: "PortfolioAudio",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioPhoto_ProfileId",
                table: "PortfolioPhoto",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioVideo_ProfileId",
                table: "PortfolioVideo",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileCollaborationGoals_ProfilesId",
                table: "ProfileCollaborationGoals",
                column: "ProfilesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileDesiredGenres_ProfilesLookingForThisGenreId",
                table: "ProfileDesiredGenres",
                column: "ProfilesLookingForThisGenreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileDesiredSpecialties_ProfilesLookingForThisSpecialtyId",
                table: "ProfileDesiredSpecialties",
                column: "ProfilesLookingForThisSpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileGenres_ProfilesId",
                table: "ProfileGenres",
                column: "ProfilesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSpecialties_SpecialtiesId",
                table: "ProfileSpecialties",
                column: "SpecialtiesId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollaborationSuggestion");

            migrationBuilder.DropTable(
                name: "EmailVerificationCode");

            migrationBuilder.DropTable(
                name: "EventRegistration");

            migrationBuilder.DropTable(
                name: "Favorite");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PortfolioAudio");

            migrationBuilder.DropTable(
                name: "PortfolioPhoto");

            migrationBuilder.DropTable(
                name: "PortfolioVideo");

            migrationBuilder.DropTable(
                name: "ProfileCollaborationGoals");

            migrationBuilder.DropTable(
                name: "ProfileDesiredGenres");

            migrationBuilder.DropTable(
                name: "ProfileDesiredSpecialties");

            migrationBuilder.DropTable(
                name: "ProfileGenres");

            migrationBuilder.DropTable(
                name: "ProfileSpecialties");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "CollaborationGoal");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "MusicalSpecialty");

            migrationBuilder.DropTable(
                name: "MusicianProfile");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
