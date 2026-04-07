using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cnss.Cotisation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToDomainEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_cot_declarations_cot_declaration_employer_identifier_cot_de~",
                schema: "cotisation",
                table: "cot_declarations");

            migrationBuilder.DropColumn(
                name: "cot_declaration_item_amount",
                schema: "cotisation",
                table: "cot_declaration_items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "cot_declaration_item_amount",
                schema: "cotisation",
                table: "cot_declaration_items",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                schema: "cotisation",
                table: "cot_declaration_items",
                keyColumn: "cot_declaration_item_identifier",
                keyValue: "DIT-0001",
                column: "cot_declaration_item_amount",
                value: 75m);

            migrationBuilder.UpdateData(
                schema: "cotisation",
                table: "cot_declaration_items",
                keyColumn: "cot_declaration_item_identifier",
                keyValue: "DIT-0002",
                column: "cot_declaration_item_amount",
                value: 100m);

            migrationBuilder.CreateIndex(
                name: "IX_cot_declarations_cot_declaration_employer_identifier_cot_de~",
                schema: "cotisation",
                table: "cot_declarations",
                columns: new[] { "cot_declaration_employer_identifier", "cot_declaration_year", "cot_declaration_month" });
        }
    }
}
