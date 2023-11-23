using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagement.Migrations
{
    public partial class AddForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_waitlists_BookId",
                table: "waitlists",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_waitlists_MemberId",
                table: "waitlists",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_borrowed_books_BookId",
                table: "borrowed_books",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_borrowed_books_MemberId",
                table: "borrowed_books",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_borrowed_books_books_BookId",
                table: "borrowed_books",
                column: "BookId",
                principalTable: "books",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_borrowed_books_members_MemberId",
                table: "borrowed_books",
                column: "MemberId",
                principalTable: "members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_waitlists_books_BookId",
                table: "waitlists",
                column: "BookId",
                principalTable: "books",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_waitlists_members_MemberId",
                table: "waitlists",
                column: "MemberId",
                principalTable: "members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_borrowed_books_books_BookId",
                table: "borrowed_books");

            migrationBuilder.DropForeignKey(
                name: "FK_borrowed_books_members_MemberId",
                table: "borrowed_books");

            migrationBuilder.DropForeignKey(
                name: "FK_waitlists_books_BookId",
                table: "waitlists");

            migrationBuilder.DropForeignKey(
                name: "FK_waitlists_members_MemberId",
                table: "waitlists");

            migrationBuilder.DropIndex(
                name: "IX_waitlists_BookId",
                table: "waitlists");

            migrationBuilder.DropIndex(
                name: "IX_waitlists_MemberId",
                table: "waitlists");

            migrationBuilder.DropIndex(
                name: "IX_borrowed_books_BookId",
                table: "borrowed_books");

            migrationBuilder.DropIndex(
                name: "IX_borrowed_books_MemberId",
                table: "borrowed_books");
        }
    }
}
