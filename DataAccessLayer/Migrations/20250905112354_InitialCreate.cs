using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hedefler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Periyot = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Onem = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Olusturma = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hedefler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Oneriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Metin = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oneriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Olusturma = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Durumlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HedefId = table.Column<int>(type: "int", nullable: true),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Saat = table.Column<TimeSpan>(type: "time", nullable: true),
                    Sure = table.Column<int>(type: "int", nullable: true),
                    Uygulandi = table.Column<bool>(type: "bit", nullable: false),
                    DurumAciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GorselYolu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Durumlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Durumlar_Hedefler_HedefId",
                        column: x => x.HedefId,
                        principalTable: "Hedefler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Oneriler",
                columns: new[] { "Id", "Aktif", "Metin" },
                values: new object[,]
                {
                    { 1, true, "Günde en az 2 kez dişlerinizi fırçalayın." },
                    { 2, true, "Diş ipi kullanmayı unutmayın." },
                    { 3, true, "Ağız gargarası kullanarak bakterileri azaltın." },
                    { 4, true, "Şekerli içecekleri sınırlayın." },
                    { 5, true, "6 ayda bir diş hekiminizi ziyaret edin." },
                    { 6, true, "Florürlü diş macunu kullanın." },
                    { 7, true, "Sigara ve tütün ürünlerinden kaçının." },
                    { 8, true, "Bol su için, ağzınızı nemli tutun." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Durumlar_HedefId",
                table: "Durumlar",
                column: "HedefId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Durumlar");

            migrationBuilder.DropTable(
                name: "Oneriler");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Hedefler");
        }
    }
}
