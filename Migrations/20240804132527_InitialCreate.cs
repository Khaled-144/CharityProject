using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CharityProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "charter",
                columns: table => new
                {
                    charter_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    charter_info = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    serial_number = table.Column<int>(type: "int", nullable: true),
                    creation_date = table.Column<DateOnly>(type: "date", nullable: false),
                    from_departement_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    to_departement_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    to_emp_id = table.Column<int>(type: "int", nullable: false),
                    receive_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_charter", x => x.charter_id);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    departement_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    departement_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    supervisor_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.departement_id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    devices_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.devices_id);
                });

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    employee_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    search_role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee", x => x.employee_id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalTransactions",
                columns: table => new
                {
                    external_transactions_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    identity_number = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    communication = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    case_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sending_party = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    receiving_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sending_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sending_number = table.Column<int>(type: "int", nullable: false),
                    receiving_number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalTransactions", x => x.external_transactions_id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    holiday_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    duration_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    allowedDuration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.holiday_id);
                });

            migrationBuilder.CreateTable(
                name: "Letters",
                columns: table => new
                {
                    letters_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    from_emp_id = table.Column<int>(type: "int", nullable: false),
                    to_emp_id = table.Column<int>(type: "int", nullable: true),
                    files = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    departement_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Letters", x => x.letters_id);
                });

            migrationBuilder.CreateTable(
                name: "OtherServices",
                columns: table => new
                {
                    service_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    service_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherServices", x => x.service_id);
                });

            migrationBuilder.CreateTable(
                name: "salaries_history",
                columns: table => new
                {
                    salaries_history_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    emp_id = table.Column<int>(type: "int", nullable: false),
                    base_salary = table.Column<double>(type: "float", nullable: false),
                    housing_allowances = table.Column<double>(type: "float", nullable: true),
                    transportaion_allowances = table.Column<double>(type: "float", nullable: true),
                    other_allowances = table.Column<double>(type: "float", nullable: true),
                    overtime = table.Column<double>(type: "float", nullable: true),
                    bonus = table.Column<double>(type: "float", nullable: true),
                    delay_discount = table.Column<double>(type: "float", nullable: true),
                    absence_discount = table.Column<double>(type: "float", nullable: true),
                    other_discount = table.Column<double>(type: "float", nullable: true),
                    debt = table.Column<double>(type: "float", nullable: true),
                    shared_portion = table.Column<double>(type: "float", nullable: true),
                    facility_portion = table.Column<double>(type: "float", nullable: true),
                    work_days = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    exchange_statement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salaries_history", x => x.salaries_history_id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    transaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    close_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    files = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    from_emp_id = table.Column<int>(type: "int", nullable: false),
                    to_emp_id = table.Column<int>(type: "int", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.transaction_id);
                });

            migrationBuilder.CreateTable(
                name: "employee_details",
                columns: table => new
                {
                    employee_details_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    identity_number = table.Column<int>(type: "int", nullable: false),
                    departement_id = table.Column<int>(type: "int", nullable: false),
                    position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    permission_position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    contract_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    national_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    education_level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hire_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    leave_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    active = table.Column<bool>(type: "bit", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    files = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employee_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_details", x => x.employee_details_id);
                    table.ForeignKey(
                        name: "FK_employee_details_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "holidays_history",
                columns: table => new
                {
                    holidays_history_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    holiday_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    duration = table.Column<int>(type: "int", nullable: false),
                    emp_id = table.Column<int>(type: "int", nullable: false),
                    creation_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    files = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holidays_history", x => x.holidays_history_id);
                    table.ForeignKey(
                        name: "FK_holidays_history_Holidays_holiday_id",
                        column: x => x.holiday_id,
                        principalTable: "Holidays",
                        principalColumn: "holiday_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Referrals",
                columns: table => new
                {
                    referral_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    transaction_id = table.Column<int>(type: "int", nullable: false),
                    from_employee_id = table.Column<int>(type: "int", nullable: false),
                    to_employee_id = table.Column<int>(type: "int", nullable: false),
                    referral_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referrals", x => x.referral_id);
                    table.ForeignKey(
                        name: "FK_Referrals_Transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "Transactions",
                        principalColumn: "transaction_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Referrals_employee_from_employee_id",
                        column: x => x.from_employee_id,
                        principalTable: "employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Referrals_employee_to_employee_id",
                        column: x => x.to_employee_id,
                        principalTable: "employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employee_details_employee_id",
                table: "employee_details",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_holidays_history_holiday_id",
                table: "holidays_history",
                column: "holiday_id");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_from_employee_id",
                table: "Referrals",
                column: "from_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_to_employee_id",
                table: "Referrals",
                column: "to_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_transaction_id",
                table: "Referrals",
                column: "transaction_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "charter");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "employee_details");

            migrationBuilder.DropTable(
                name: "ExternalTransactions");

            migrationBuilder.DropTable(
                name: "holidays_history");

            migrationBuilder.DropTable(
                name: "Letters");

            migrationBuilder.DropTable(
                name: "OtherServices");

            migrationBuilder.DropTable(
                name: "Referrals");

            migrationBuilder.DropTable(
                name: "salaries_history");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "employee");
        }
    }
}
