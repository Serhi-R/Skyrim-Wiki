using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyrimWIKI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CommentAuthorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Comments",
                type: "TEXT",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Comments");
        }
    }
}
