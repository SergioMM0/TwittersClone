using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowersService.Migrations
{
    /// <inheritdoc />
    public partial class FollowerUpdatedForNotif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ListenToNotifications",
                table: "FollowersTable",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListenToNotifications",
                table: "FollowersTable");
        }
    }
}
