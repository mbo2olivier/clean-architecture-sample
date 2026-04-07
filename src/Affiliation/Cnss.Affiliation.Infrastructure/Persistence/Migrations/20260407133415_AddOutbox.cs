using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cnss.Affiliation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aff_outbox_messages",
                schema: "affiliation",
                columns: table => new
                {
                    aff_outbox_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    aff_outbox_event_type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    aff_outbox_routing_key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    aff_outbox_payload = table.Column<string>(type: "jsonb", nullable: false),
                    aff_outbox_occurred_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    aff_outbox_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    aff_outbox_attempt_count = table.Column<int>(type: "integer", nullable: false),
                    aff_outbox_processing_started_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    aff_outbox_locked_until_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    aff_outbox_processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    aff_outbox_last_error = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aff_outbox_messages", x => x.aff_outbox_message_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aff_outbox_messages_aff_outbox_locked_until_utc",
                schema: "affiliation",
                table: "aff_outbox_messages",
                column: "aff_outbox_locked_until_utc");

            migrationBuilder.CreateIndex(
                name: "IX_aff_outbox_messages_aff_outbox_status_aff_outbox_occurred_o~",
                schema: "affiliation",
                table: "aff_outbox_messages",
                columns: new[] { "aff_outbox_status", "aff_outbox_occurred_on_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aff_outbox_messages",
                schema: "affiliation");
        }
    }
}
