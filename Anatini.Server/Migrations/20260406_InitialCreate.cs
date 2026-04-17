using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Anatini.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "channels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    about = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    about = table.Column<string>(type: "text", nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "channel_handles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channel_handles", x => x.id);
                    table.ForeignKey(
                        name: "fk_channel_handles_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "channel_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    blob_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    blob_container_name = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    alt_text = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channel_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_channel_images_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    article = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_notes_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    date_nz = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_posts", x => x.id);
                    table.ForeignKey(
                        name: "fk_posts_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: true),
                    event_type = table.Column<int>(type: "integer", nullable: false),
                    ip_address = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    meta_data = table.Column<string>(type: "jsonb", nullable: true),
                    date_time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_logs_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_channels",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_channels", x => new { x.user_id, x.channel_id });
                    table.ForeignKey(
                        name: "fk_user_channels_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_channels_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claims_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_emails",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    confirmation_code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_emails", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_emails_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_handles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_handles", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_handles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    blob_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    blob_container_name = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    alt_text = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_images_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_user_logins_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_user_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_trusts",
                columns: table => new
                {
                    source_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_trusts", x => new { x.source_user_id, x.target_user_id });
                    table.ForeignKey(
                        name: "fk_user_trusts_users_source_user_id",
                        column: x => x.source_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_trusts_users_target_user_id",
                        column: x => x.target_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "note_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    note_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    blob_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    blob_container_name = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    alt_text = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_note_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_note_images_notes_note_id",
                        column: x => x.note_id,
                        principalTable: "notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "post_handles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_handles", x => x.id);
                    table.ForeignKey(
                        name: "fk_post_handles_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_post_handles_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "post_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    blob_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    blob_container_name = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    alt_text = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_post_images_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "post_versions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    article = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_versions", x => x.id);
                    table.ForeignKey(
                        name: "fk_post_versions_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_channel_handles_channel_id",
                table: "channel_handles",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_channel_handles_handle",
                table: "channel_handles",
                column: "handle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_channel_images_channel_id_handle",
                table: "channel_images",
                columns: new[] { "channel_id", "handle" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_channels_handle",
                table: "channels",
                column: "handle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_logs_channel_id",
                table: "logs",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_logs_date_time_utc",
                table: "logs",
                column: "date_time_utc");

            migrationBuilder.CreateIndex(
                name: "ix_logs_event_type",
                table: "logs",
                column: "event_type");

            migrationBuilder.CreateIndex(
                name: "ix_logs_user_id",
                table: "logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_note_images_note_id_handle",
                table: "note_images",
                columns: new[] { "note_id", "handle" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notes_channel_id_created_at_utc",
                table: "notes",
                columns: new[] { "channel_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_notes_channel_id_handle",
                table: "notes",
                columns: new[] { "channel_id", "handle" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notes_created_at_utc",
                table: "notes",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_post_handles_channel_id_handle",
                table: "post_handles",
                columns: new[] { "channel_id", "handle" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_post_handles_post_id",
                table: "post_handles",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_images_post_id_handle",
                table: "post_images",
                columns: new[] { "post_id", "handle" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_post_versions_post_id_handle",
                table: "post_versions",
                columns: new[] { "post_id", "handle" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_posts_channel_id_date_nz",
                table: "posts",
                columns: new[] { "channel_id", "date_nz" });

            migrationBuilder.CreateIndex(
                name: "ix_posts_channel_id_handle",
                table: "posts",
                columns: new[] { "channel_id", "handle" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_published_posts_date_nz",
                table: "posts",
                column: "date_nz",
                filter: "status = 1");

            migrationBuilder.CreateIndex(
                name: "ix_role_claims_role_id",
                table: "role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_normalized_name",
                table: "roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_channels_channel_id",
                table: "user_channels",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_claims_user_id",
                table: "user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_emails_normalized_email",
                table: "user_emails",
                column: "normalized_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_emails_null_user_id_created_at_utc",
                table: "user_emails",
                columns: new[] { "user_id", "created_at_utc" },
                filter: "user_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_user_handles_handle",
                table: "user_handles",
                column: "handle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_handles_user_id",
                table: "user_handles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_images_user_id_handle",
                table: "user_images",
                columns: new[] { "user_id", "handle" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_logins_user_id",
                table: "user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_trusts_target_user_id_source_user_id",
                table: "user_trusts",
                columns: new[] { "target_user_id", "source_user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_users_handle",
                table: "users",
                column: "handle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_normalized_email",
                table: "users",
                column: "normalized_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_normalized_user_name",
                table: "users",
                column: "normalized_user_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "channel_handles");

            migrationBuilder.DropTable(
                name: "channel_images");

            migrationBuilder.DropTable(
                name: "logs");

            migrationBuilder.DropTable(
                name: "note_images");

            migrationBuilder.DropTable(
                name: "post_handles");

            migrationBuilder.DropTable(
                name: "post_images");

            migrationBuilder.DropTable(
                name: "post_versions");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "user_channels");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_emails");

            migrationBuilder.DropTable(
                name: "user_handles");

            migrationBuilder.DropTable(
                name: "user_images");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "user_trusts");

            migrationBuilder.DropTable(
                name: "notes");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "channels");
        }
    }
}
