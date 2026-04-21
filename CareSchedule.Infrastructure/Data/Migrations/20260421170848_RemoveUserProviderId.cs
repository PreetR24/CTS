using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareSchedule.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserProviderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                table: "User",
                type: "int",
                nullable: true);
        }
    }
}
