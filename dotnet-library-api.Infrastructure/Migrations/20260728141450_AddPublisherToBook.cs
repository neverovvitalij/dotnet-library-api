using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_library_api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublisherToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "Books",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "Books");
        }
    }
}
