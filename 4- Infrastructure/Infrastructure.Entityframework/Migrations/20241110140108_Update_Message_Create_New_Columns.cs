using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Entityframework.Migrations
{
    /// <inheritdoc />
    public partial class Update_Message_Create_New_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdMessageTelegram",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdMessageTelegram",
                table: "Messages");
        }
    }
}
