using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTenancy.Data.Migrations {
    public partial class Enterprises : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "Enterprises",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserCreationId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Enterprises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enterprises_AspNetUsers_UserCreationId",
                        column: x => x.UserCreationId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EnterpriseUserPermissions",
                columns: table => new {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EnterpriseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Permission = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_EnterpriseUserPermissions", x => new { x.EnterpriseId, x.UserId, x.Permission });
                    table.ForeignKey(
                        name: "FK_EnterpriseUserPermissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnterpriseUserPermissions_Enterprises_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalTable: "Enterprises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vinculations",
                columns: table => new {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnterpriseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Vinculations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vinculations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vinculations_Enterprises_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalTable: "Enterprises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enterprises_UserCreationId",
                table: "Enterprises",
                column: "UserCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_EnterpriseUserPermissions_UserId",
                table: "EnterpriseUserPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vinculations_EnterpriseId",
                table: "Vinculations",
                column: "EnterpriseId");

            migrationBuilder.CreateIndex(
                name: "IX_Vinculations_UserId",
                table: "Vinculations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "EnterpriseUserPermissions");

            migrationBuilder.DropTable(
                name: "Vinculations");

            migrationBuilder.DropTable(
                name: "Enterprises");
        }
    }
}
