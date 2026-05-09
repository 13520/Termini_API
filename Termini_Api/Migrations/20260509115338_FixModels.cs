using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Termini_Api.Migrations
{
    /// <inheritdoc />
    public partial class FixModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Terens_Cities_CityId",
                table: "Terens");

            migrationBuilder.DropForeignKey(
                name: "FK_Terens_Sports_SportId",
                table: "Terens");

            migrationBuilder.DropForeignKey(
                name: "FK_Terens_Users_ClientUserId",
                table: "Terens");

            migrationBuilder.DropForeignKey(
                name: "FK_Termins_Terens_TerenId",
                table: "Termins");

            migrationBuilder.DropForeignKey(
                name: "FK_Termins_Users_BeneficiaryUserId",
                table: "Termins");

            migrationBuilder.DropIndex(
                name: "IX_Termins_BeneficiaryUserId",
                table: "Termins");

            migrationBuilder.DropIndex(
                name: "IX_Terens_ClientUserId",
                table: "Terens");

            migrationBuilder.DropColumn(
                name: "BeneficiaryId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BeneficiaryUserId",
                table: "Termins");

            migrationBuilder.DropColumn(
                name: "ClientUserId",
                table: "Terens");

            migrationBuilder.AlterColumn<long>(
                name: "TerenId",
                table: "Termins",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "BeneficiaryId",
                table: "Termins",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SportId",
                table: "Terens",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Terens",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                table: "Terens",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageBase64",
                table: "Terens",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Termins_BeneficiaryId",
                table: "Termins",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Terens_ClientId",
                table: "Terens",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Terens_Cities_CityId",
                table: "Terens",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Terens_Sports_SportId",
                table: "Terens",
                column: "SportId",
                principalTable: "Sports",
                principalColumn: "SportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Terens_Users_ClientId",
                table: "Terens",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Termins_Terens_TerenId",
                table: "Termins",
                column: "TerenId",
                principalTable: "Terens",
                principalColumn: "TerenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Termins_Users_BeneficiaryId",
                table: "Termins",
                column: "BeneficiaryId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Terens_Cities_CityId",
                table: "Terens");

            migrationBuilder.DropForeignKey(
                name: "FK_Terens_Sports_SportId",
                table: "Terens");

            migrationBuilder.DropForeignKey(
                name: "FK_Terens_Users_ClientId",
                table: "Terens");

            migrationBuilder.DropForeignKey(
                name: "FK_Termins_Terens_TerenId",
                table: "Termins");

            migrationBuilder.DropForeignKey(
                name: "FK_Termins_Users_BeneficiaryId",
                table: "Termins");

            migrationBuilder.DropIndex(
                name: "IX_Termins_BeneficiaryId",
                table: "Termins");

            migrationBuilder.DropIndex(
                name: "IX_Terens_ClientId",
                table: "Terens");

            migrationBuilder.DropColumn(
                name: "BeneficiaryId",
                table: "Termins");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Terens");

            migrationBuilder.DropColumn(
                name: "ImageBase64",
                table: "Terens");

            migrationBuilder.AddColumn<long>(
                name: "BeneficiaryId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TerenId",
                table: "Termins",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BeneficiaryUserId",
                table: "Termins",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<int>(
                name: "SportId",
                table: "Terens",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Terens",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ClientUserId",
                table: "Terens",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Termins_BeneficiaryUserId",
                table: "Termins",
                column: "BeneficiaryUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Terens_ClientUserId",
                table: "Terens",
                column: "ClientUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Terens_Cities_CityId",
                table: "Terens",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Terens_Sports_SportId",
                table: "Terens",
                column: "SportId",
                principalTable: "Sports",
                principalColumn: "SportId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Terens_Users_ClientUserId",
                table: "Terens",
                column: "ClientUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Termins_Terens_TerenId",
                table: "Termins",
                column: "TerenId",
                principalTable: "Terens",
                principalColumn: "TerenId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Termins_Users_BeneficiaryUserId",
                table: "Termins",
                column: "BeneficiaryUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
