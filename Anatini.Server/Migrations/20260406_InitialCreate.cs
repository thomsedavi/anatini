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
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "spaces",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    about = table.Column<string>(type: "character varying(511)", maxLength: 511, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spaces", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
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
                    normalized_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
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
                name: "space_handles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    space_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_space_handles", x => x.id);
                    table.ForeignKey(
                        name: "fk_space_handles_spaces_space_id",
                        column: x => x.space_id,
                        principalTable: "spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "space_images",
                columns: table => new
                {
                    space_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    blob_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    blob_container_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    alt_text = table.Column<string>(type: "character varying(511)", maxLength: 511, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_space_images", x => new { x.space_id, x.handle });
                    table.ForeignKey(
                        name: "fk_space_images_spaces_space_id",
                        column: x => x.space_id,
                        principalTable: "spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    space_id = table.Column<Guid>(type: "uuid", nullable: true),
                    handle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    published_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    article = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "character varying(2047)", maxLength: 2047, nullable: true),
                    current_version_number = table.Column<int>(type: "integer", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contents", x => x.id);
                    table.CheckConstraint("ck_contents_user_id_xor_space_id", "(user_id IS NULL AND space_id IS NOT NULL) OR (space_id IS NULL AND user_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_contents_spaces_space_id",
                        column: x => x.space_id,
                        principalTable: "spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_contents_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event_series",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    space_id = table.Column<Guid>(type: "uuid", nullable: true),
                    handle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    article = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "character varying(2047)", maxLength: 2047, nullable: true),
                    starts_at_nz = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ends_at_nz = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    recurrence_rule = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    expires_at_nz = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_event_series", x => x.id);
                    table.CheckConstraint("ck_event_series_user_id_xor_space_id", "(user_id IS NULL AND space_id IS NOT NULL) OR (space_id IS NULL AND user_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_event_series_spaces_space_id",
                        column: x => x.space_id,
                        principalTable: "spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_event_series_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    space_id = table.Column<Guid>(type: "uuid", nullable: true),
                    event_type = table.Column<int>(type: "integer", nullable: false),
                    ip_address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    meta_data = table.Column<string>(type: "jsonb", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_logs_spaces_space_id",
                        column: x => x.space_id,
                        principalTable: "spaces",
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
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    confirmation_code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    normalized_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
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
                    handle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
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
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    blob_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    blob_container_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    alt_text = table.Column<string>(type: "character varying(511)", maxLength: 511, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_images", x => new { x.user_id, x.handle });
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
                name: "user_space_edges",
                columns: table => new
                {
                    source_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_space_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_space_edges", x => new { x.source_user_id, x.target_space_id, x.label });
                    table.ForeignKey(
                        name: "fk_user_space_edges_spaces_target_space_id",
                        column: x => x.target_space_id,
                        principalTable: "spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_space_edges_users_source_user_id",
                        column: x => x.source_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "user_user_edges",
                columns: table => new
                {
                    source_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_user_edges", x => new { x.source_user_id, x.target_user_id, x.label });
                    table.ForeignKey(
                        name: "fk_user_user_edges_users_source_user_id",
                        column: x => x.source_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_user_edges_users_target_user_id",
                        column: x => x.target_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "content_images",
                columns: table => new
                {
                    content_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    blob_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    blob_container_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    alt_text = table.Column<string>(type: "character varying(511)", maxLength: 511, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_images", x => new { x.content_id, x.handle });
                    table.ForeignKey(
                        name: "fk_content_images_contents_content_id",
                        column: x => x.content_id,
                        principalTable: "contents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "content_versions",
                columns: table => new
                {
                    content_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version_number = table.Column<int>(type: "integer", nullable: false),
                    article = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_versions", x => new { x.content_id, x.version_number });
                    table.ForeignKey(
                        name: "fk_content_versions_contents_content_id",
                        column: x => x.content_id,
                        principalTable: "contents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_content_edges",
                columns: table => new
                {
                    source_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_content_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_content_edges", x => new { x.source_user_id, x.target_content_id, x.label });
                    table.ForeignKey(
                        name: "fk_user_content_edges_contents_target_content_id",
                        column: x => x.target_content_id,
                        principalTable: "contents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_content_edges_users_source_user_id",
                        column: x => x.source_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event_exceptions",
                columns: table => new
                {
                    event_series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_starts_at_nz = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_cancelled = table.Column<bool>(type: "boolean", nullable: false),
                    override_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    override_article = table.Column<string>(type: "text", nullable: true),
                    override_url = table.Column<string>(type: "character varying(2047)", maxLength: 2047, nullable: true),
                    override_starts_at_nz = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    override_duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    override_ends_at_nz = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_event_exceptions", x => new { x.event_series_id, x.target_starts_at_nz });
                    table.ForeignKey(
                        name: "fk_event_exceptions_event_series_event_series_id",
                        column: x => x.event_series_id,
                        principalTable: "event_series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_instances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    space_id = table.Column<Guid>(type: "uuid", nullable: true),
                    handle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    article = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "character varying(2047)", maxLength: 2047, nullable: true),
                    starts_at_nz = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ends_at_nz = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_event_instances", x => x.id);
                    table.CheckConstraint("ck_event_instances_user_id_xor_space_id", "(user_id IS NULL AND space_id IS NOT NULL) OR (space_id IS NULL AND user_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_event_instances_event_series_event_series_id",
                        column: x => x.event_series_id,
                        principalTable: "event_series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_event_instances_spaces_space_id",
                        column: x => x.space_id,
                        principalTable: "spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_event_instances_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contents_space_id_type_handle",
                table: "contents",
                columns: new[] { "space_id", "type", "handle" },
                unique: true,
                filter: "space_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_contents_user_id_type_handle",
                table: "contents",
                columns: new[] { "user_id", "type", "handle" },
                unique: true,
                filter: "user_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_published_contents_date_nz",
                table: "contents",
                column: "published_at_utc",
                filter: "status = 1");

            migrationBuilder.CreateIndex(
                name: "ix_event_instances_event_series_id",
                table: "event_instances",
                column: "event_series_id");

            migrationBuilder.CreateIndex(
                name: "ix_event_instances_space_id",
                table: "event_instances",
                column: "space_id");

            migrationBuilder.CreateIndex(
                name: "ix_event_instances_user_id",
                table: "event_instances",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_event_series_space_id",
                table: "event_series",
                column: "space_id");

            migrationBuilder.CreateIndex(
                name: "ix_event_series_user_id",
                table: "event_series",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_logs_created_at_utc",
                table: "logs",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_logs_event_type",
                table: "logs",
                column: "event_type");

            migrationBuilder.CreateIndex(
                name: "ix_logs_space_id",
                table: "logs",
                column: "space_id");

            migrationBuilder.CreateIndex(
                name: "ix_logs_user_id",
                table: "logs",
                column: "user_id");

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
                name: "ix_space_handles_handle",
                table: "space_handles",
                column: "handle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_space_handles_space_id",
                table: "space_handles",
                column: "space_id");

            migrationBuilder.CreateIndex(
                name: "ix_spaces_handle",
                table: "spaces",
                column: "handle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_claims_user_id",
                table: "user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_content_edges_target_content_id_label_source_user_id",
                table: "user_content_edges",
                columns: new[] { "target_content_id", "label", "source_user_id" });

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
                name: "ix_user_logins_user_id",
                table: "user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_space_edges_target_space_id_label_source_user_id",
                table: "user_space_edges",
                columns: new[] { "target_space_id", "label", "source_user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_user_user_edges_target_user_id_label_source_user_id",
                table: "user_user_edges",
                columns: new[] { "target_user_id", "label", "source_user_id" });

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
                name: "content_images");

            migrationBuilder.DropTable(
                name: "content_versions");

            migrationBuilder.DropTable(
                name: "event_exceptions");

            migrationBuilder.DropTable(
                name: "event_instances");

            migrationBuilder.DropTable(
                name: "logs");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "space_handles");

            migrationBuilder.DropTable(
                name: "space_images");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_content_edges");

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
                name: "user_space_edges");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "user_user_edges");

            migrationBuilder.DropTable(
                name: "event_series");

            migrationBuilder.DropTable(
                name: "contents");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "spaces");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
