using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cnss.Cotisation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cot_outbox_messages",
                schema: "cotisation",
                columns: table => new
                {
                    cot_outbox_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cot_outbox_event_type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    cot_outbox_routing_key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cot_outbox_payload = table.Column<string>(type: "jsonb", nullable: false),
                    cot_outbox_occurred_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cot_outbox_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cot_outbox_attempt_count = table.Column<int>(type: "integer", nullable: false),
                    cot_outbox_processing_started_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cot_outbox_locked_until_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cot_outbox_processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cot_outbox_last_error = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cot_outbox_messages", x => x.cot_outbox_message_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cot_outbox_messages_cot_outbox_locked_until_utc",
                schema: "cotisation",
                table: "cot_outbox_messages",
                column: "cot_outbox_locked_until_utc");

            migrationBuilder.CreateIndex(
                name: "IX_cot_outbox_messages_cot_outbox_status_cot_outbox_occurred_o~",
                schema: "cotisation",
                table: "cot_outbox_messages",
                columns: new[] { "cot_outbox_status", "cot_outbox_occurred_on_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cot_outbox_messages",
                schema: "cotisation");
        }
    }
}
