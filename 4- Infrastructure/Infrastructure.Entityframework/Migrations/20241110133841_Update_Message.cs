using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Entityframework.Migrations
{
    /// <inheritdoc />
    public partial class Update_Message : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatIdChat",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ChatIdChat",
                table: "Messages",
                newName: "ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ChatIdChat",
                table: "Messages",
                newName: "IX_Messages_ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "IdChat",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "Messages",
                newName: "ChatIdChat");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                newName: "IX_Messages_ChatIdChat");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatIdChat",
                table: "Messages",
                column: "ChatIdChat",
                principalTable: "Chats",
                principalColumn: "IdChat",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
