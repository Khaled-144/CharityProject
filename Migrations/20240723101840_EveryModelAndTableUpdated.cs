using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CharityProject.Migrations
{
    /// <inheritdoc />
    public partial class EveryModelAndTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HolidayHistories_Holidays_HolidayId",
                table: "HolidayHistories");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Charters",
                table: "Charters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HolidayHistories",
                table: "HolidayHistories");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "OtherServices");

            migrationBuilder.RenameTable(
                name: "HolidayHistories",
                newName: "holidays_history");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Transactions",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Transactions",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Files",
                table: "Transactions",
                newName: "files");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Transactions",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "ToEmpId",
                table: "Transactions",
                newName: "to_emp_id");

            migrationBuilder.RenameColumn(
                name: "FromEmpId",
                table: "Transactions",
                newName: "from_emp_id");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Transactions",
                newName: "department_id");

            migrationBuilder.RenameColumn(
                name: "CloseDate",
                table: "Transactions",
                newName: "create_date");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Transactions",
                newName: "transaction_id");

            migrationBuilder.RenameColumn(
                name: "Overtime",
                table: "SalaryHistories",
                newName: "overtime");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "SalaryHistories",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Debt",
                table: "SalaryHistories",
                newName: "debt");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "SalaryHistories",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Bonus",
                table: "SalaryHistories",
                newName: "bonus");

            migrationBuilder.RenameColumn(
                name: "WorkDays",
                table: "SalaryHistories",
                newName: "work_days");

            migrationBuilder.RenameColumn(
                name: "TransportationAllowances",
                table: "SalaryHistories",
                newName: "transportaion_allowances");

            migrationBuilder.RenameColumn(
                name: "SharedPortion",
                table: "SalaryHistories",
                newName: "shared_portion");

            migrationBuilder.RenameColumn(
                name: "OtherDiscount",
                table: "SalaryHistories",
                newName: "other_discount");

            migrationBuilder.RenameColumn(
                name: "OtherAllowances",
                table: "SalaryHistories",
                newName: "other_allowances");

            migrationBuilder.RenameColumn(
                name: "HousingAllowances",
                table: "SalaryHistories",
                newName: "housing_allowances");

            migrationBuilder.RenameColumn(
                name: "FacilityPortion",
                table: "SalaryHistories",
                newName: "facility_portion");

            migrationBuilder.RenameColumn(
                name: "ExchangeStatement",
                table: "SalaryHistories",
                newName: "exchange_statement");

            migrationBuilder.RenameColumn(
                name: "EmpId",
                table: "SalaryHistories",
                newName: "emp_id");

            migrationBuilder.RenameColumn(
                name: "DelayDiscount",
                table: "SalaryHistories",
                newName: "delay_discount");

            migrationBuilder.RenameColumn(
                name: "BaseSalary",
                table: "SalaryHistories",
                newName: "base_salary");

            migrationBuilder.RenameColumn(
                name: "AbsenceDiscount",
                table: "SalaryHistories",
                newName: "absence_discount");

            migrationBuilder.RenameColumn(
                name: "SalaryHistoryId",
                table: "SalaryHistories",
                newName: "salaries_history_id");

            migrationBuilder.RenameColumn(
                name: "ServiceName",
                table: "OtherServices",
                newName: "service_name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OtherServices",
                newName: "service_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Letters",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Files",
                table: "Letters",
                newName: "files");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Letters",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Letters",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "ToEmpId",
                table: "Letters",
                newName: "to_emp_id");

            migrationBuilder.RenameColumn(
                name: "FromEmpId",
                table: "Letters",
                newName: "from_emp_id");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Letters",
                newName: "departement_id");

            migrationBuilder.RenameColumn(
                name: "LetterId",
                table: "Letters",
                newName: "letters_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Holidays",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Holidays",
                newName: "duration");

            migrationBuilder.RenameColumn(
                name: "AllowedDuration",
                table: "Holidays",
                newName: "allowedDuration");

            migrationBuilder.RenameColumn(
                name: "DurationUnit",
                table: "Holidays",
                newName: "duration_Unit");

            migrationBuilder.RenameColumn(
                name: "HolidayId",
                table: "Holidays",
                newName: "holiday_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ExternalTransactions",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ExternalTransactions",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Communication",
                table: "ExternalTransactions",
                newName: "communication");

            migrationBuilder.RenameColumn(
                name: "SendingParty",
                table: "ExternalTransactions",
                newName: "sending_party");

            migrationBuilder.RenameColumn(
                name: "SendingNumber",
                table: "ExternalTransactions",
                newName: "sending_number");

            migrationBuilder.RenameColumn(
                name: "SendingDate",
                table: "ExternalTransactions",
                newName: "sending_date");

            migrationBuilder.RenameColumn(
                name: "ReceivingNumber",
                table: "ExternalTransactions",
                newName: "receiving_number");

            migrationBuilder.RenameColumn(
                name: "ReceivingDate",
                table: "ExternalTransactions",
                newName: "receiving_date");

            migrationBuilder.RenameColumn(
                name: "IdentityNumber",
                table: "ExternalTransactions",
                newName: "identity_number");

            migrationBuilder.RenameColumn(
                name: "CaseStatus",
                table: "ExternalTransactions",
                newName: "case_status");

            migrationBuilder.RenameColumn(
                name: "ExternalTransactionId",
                table: "ExternalTransactions",
                newName: "external_transactions_id");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Employees",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Employees",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Employees",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "SearchRole",
                table: "Employees",
                newName: "search_role");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Employees",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "EmployeeDetails",
                newName: "position");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "EmployeeDetails",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "Files",
                table: "EmployeeDetails",
                newName: "files");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "EmployeeDetails",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "EmployeeDetails",
                newName: "active");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "EmployeeDetails",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "PermissionPosition",
                table: "EmployeeDetails",
                newName: "permission_position");

            migrationBuilder.RenameColumn(
                name: "NationalAddress",
                table: "EmployeeDetails",
                newName: "national_address");

            migrationBuilder.RenameColumn(
                name: "LeaveDate",
                table: "EmployeeDetails",
                newName: "leave_date");

            migrationBuilder.RenameColumn(
                name: "IdentityNumber",
                table: "EmployeeDetails",
                newName: "identity_number");

            migrationBuilder.RenameColumn(
                name: "HireDate",
                table: "EmployeeDetails",
                newName: "hire_date");

            migrationBuilder.RenameColumn(
                name: "EducationLevel",
                table: "EmployeeDetails",
                newName: "education_level");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "EmployeeDetails",
                newName: "departement_id");

            migrationBuilder.RenameColumn(
                name: "ContractType",
                table: "EmployeeDetails",
                newName: "contract_type");

            migrationBuilder.RenameColumn(
                name: "EmployeeDetailsId",
                table: "EmployeeDetails",
                newName: "employee_details_id");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Devices",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Devices",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "DeviceId",
                table: "Devices",
                newName: "devices_id");

            migrationBuilder.RenameColumn(
                name: "SupervisorId",
                table: "Departments",
                newName: "supervisor_id");

            migrationBuilder.RenameColumn(
                name: "DepartmentName",
                table: "Departments",
                newName: "departement_name");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Departments",
                newName: "departement_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Charters",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Devices",
                table: "Charters",
                newName: "devices");

            migrationBuilder.RenameColumn(
                name: "ToEmpId",
                table: "Charters",
                newName: "to_emp_id");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Charters",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Charters",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "CharterId",
                table: "Charters",
                newName: "number_of_devices");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "holidays_history",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "holidays_history",
                newName: "duration");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "holidays_history",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "HolidayId",
                table: "holidays_history",
                newName: "holiday_id");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "holidays_history",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "EmpId",
                table: "holidays_history",
                newName: "emp_id");

            migrationBuilder.RenameColumn(
                name: "HolidayHistoryId",
                table: "holidays_history",
                newName: "holidays_history_id");

            migrationBuilder.RenameIndex(
                name: "IX_HolidayHistories_HolidayId",
                table: "holidays_history",
                newName: "IX_holidays_history_holiday_id");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "close_date",
                table: "Transactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "Letters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "number_of_devices",
                table: "Charters",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "charter_id",
                table: "Charters",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "holidays_history",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateOnly>(
                name: "creation_date",
                table: "holidays_history",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "holidays_history",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "files",
                table: "holidays_history",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "holidays_history",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Charters",
                table: "Charters",
                column: "charter_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_holidays_history",
                table: "holidays_history",
                column: "holidays_history_id");

            migrationBuilder.AddForeignKey(
                name: "FK_holidays_history_Holidays_holiday_id",
                table: "holidays_history",
                column: "holiday_id",
                principalTable: "Holidays",
                principalColumn: "holiday_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_holidays_history_Holidays_holiday_id",
                table: "holidays_history");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Charters",
                table: "Charters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_holidays_history",
                table: "holidays_history");

            migrationBuilder.DropColumn(
                name: "close_date",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "type",
                table: "Letters");

            migrationBuilder.DropColumn(
                name: "charter_id",
                table: "Charters");

            migrationBuilder.DropColumn(
                name: "creation_date",
                table: "holidays_history");

            migrationBuilder.DropColumn(
                name: "description",
                table: "holidays_history");

            migrationBuilder.DropColumn(
                name: "files",
                table: "holidays_history");

            migrationBuilder.DropColumn(
                name: "title",
                table: "holidays_history");

            migrationBuilder.RenameTable(
                name: "holidays_history",
                newName: "HolidayHistories");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Transactions",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Transactions",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "files",
                table: "Transactions",
                newName: "Files");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Transactions",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "to_emp_id",
                table: "Transactions",
                newName: "ToEmpId");

            migrationBuilder.RenameColumn(
                name: "from_emp_id",
                table: "Transactions",
                newName: "FromEmpId");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "Transactions",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "create_date",
                table: "Transactions",
                newName: "CloseDate");

            migrationBuilder.RenameColumn(
                name: "transaction_id",
                table: "Transactions",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "overtime",
                table: "SalaryHistories",
                newName: "Overtime");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "SalaryHistories",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "debt",
                table: "SalaryHistories",
                newName: "Debt");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "SalaryHistories",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "bonus",
                table: "SalaryHistories",
                newName: "Bonus");

            migrationBuilder.RenameColumn(
                name: "work_days",
                table: "SalaryHistories",
                newName: "WorkDays");

            migrationBuilder.RenameColumn(
                name: "transportaion_allowances",
                table: "SalaryHistories",
                newName: "TransportationAllowances");

            migrationBuilder.RenameColumn(
                name: "shared_portion",
                table: "SalaryHistories",
                newName: "SharedPortion");

            migrationBuilder.RenameColumn(
                name: "other_discount",
                table: "SalaryHistories",
                newName: "OtherDiscount");

            migrationBuilder.RenameColumn(
                name: "other_allowances",
                table: "SalaryHistories",
                newName: "OtherAllowances");

            migrationBuilder.RenameColumn(
                name: "housing_allowances",
                table: "SalaryHistories",
                newName: "HousingAllowances");

            migrationBuilder.RenameColumn(
                name: "facility_portion",
                table: "SalaryHistories",
                newName: "FacilityPortion");

            migrationBuilder.RenameColumn(
                name: "exchange_statement",
                table: "SalaryHistories",
                newName: "ExchangeStatement");

            migrationBuilder.RenameColumn(
                name: "emp_id",
                table: "SalaryHistories",
                newName: "EmpId");

            migrationBuilder.RenameColumn(
                name: "delay_discount",
                table: "SalaryHistories",
                newName: "DelayDiscount");

            migrationBuilder.RenameColumn(
                name: "base_salary",
                table: "SalaryHistories",
                newName: "BaseSalary");

            migrationBuilder.RenameColumn(
                name: "absence_discount",
                table: "SalaryHistories",
                newName: "AbsenceDiscount");

            migrationBuilder.RenameColumn(
                name: "salaries_history_id",
                table: "SalaryHistories",
                newName: "SalaryHistoryId");

            migrationBuilder.RenameColumn(
                name: "service_name",
                table: "OtherServices",
                newName: "ServiceName");

            migrationBuilder.RenameColumn(
                name: "service_id",
                table: "OtherServices",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Letters",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "files",
                table: "Letters",
                newName: "Files");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Letters",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Letters",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "to_emp_id",
                table: "Letters",
                newName: "ToEmpId");

            migrationBuilder.RenameColumn(
                name: "from_emp_id",
                table: "Letters",
                newName: "FromEmpId");

            migrationBuilder.RenameColumn(
                name: "departement_id",
                table: "Letters",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "letters_id",
                table: "Letters",
                newName: "LetterId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Holidays",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "duration",
                table: "Holidays",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "allowedDuration",
                table: "Holidays",
                newName: "AllowedDuration");

            migrationBuilder.RenameColumn(
                name: "duration_Unit",
                table: "Holidays",
                newName: "DurationUnit");

            migrationBuilder.RenameColumn(
                name: "holiday_id",
                table: "Holidays",
                newName: "HolidayId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "ExternalTransactions",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "ExternalTransactions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "communication",
                table: "ExternalTransactions",
                newName: "Communication");

            migrationBuilder.RenameColumn(
                name: "sending_party",
                table: "ExternalTransactions",
                newName: "SendingParty");

            migrationBuilder.RenameColumn(
                name: "sending_number",
                table: "ExternalTransactions",
                newName: "SendingNumber");

            migrationBuilder.RenameColumn(
                name: "sending_date",
                table: "ExternalTransactions",
                newName: "SendingDate");

            migrationBuilder.RenameColumn(
                name: "receiving_number",
                table: "ExternalTransactions",
                newName: "ReceivingNumber");

            migrationBuilder.RenameColumn(
                name: "receiving_date",
                table: "ExternalTransactions",
                newName: "ReceivingDate");

            migrationBuilder.RenameColumn(
                name: "identity_number",
                table: "ExternalTransactions",
                newName: "IdentityNumber");

            migrationBuilder.RenameColumn(
                name: "case_status",
                table: "ExternalTransactions",
                newName: "CaseStatus");

            migrationBuilder.RenameColumn(
                name: "external_transactions_id",
                table: "ExternalTransactions",
                newName: "ExternalTransactionId");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Employees",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "Employees",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Employees",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "search_role",
                table: "Employees",
                newName: "SearchRole");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "Employees",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "position",
                table: "EmployeeDetails",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "EmployeeDetails",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "files",
                table: "EmployeeDetails",
                newName: "Files");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "EmployeeDetails",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "active",
                table: "EmployeeDetails",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "EmployeeDetails",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "permission_position",
                table: "EmployeeDetails",
                newName: "PermissionPosition");

            migrationBuilder.RenameColumn(
                name: "national_address",
                table: "EmployeeDetails",
                newName: "NationalAddress");

            migrationBuilder.RenameColumn(
                name: "leave_date",
                table: "EmployeeDetails",
                newName: "LeaveDate");

            migrationBuilder.RenameColumn(
                name: "identity_number",
                table: "EmployeeDetails",
                newName: "IdentityNumber");

            migrationBuilder.RenameColumn(
                name: "hire_date",
                table: "EmployeeDetails",
                newName: "HireDate");

            migrationBuilder.RenameColumn(
                name: "education_level",
                table: "EmployeeDetails",
                newName: "EducationLevel");

            migrationBuilder.RenameColumn(
                name: "departement_id",
                table: "EmployeeDetails",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "contract_type",
                table: "EmployeeDetails",
                newName: "ContractType");

            migrationBuilder.RenameColumn(
                name: "employee_details_id",
                table: "EmployeeDetails",
                newName: "EmployeeDetailsId");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "Devices",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Devices",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "devices_id",
                table: "Devices",
                newName: "DeviceId");

            migrationBuilder.RenameColumn(
                name: "supervisor_id",
                table: "Departments",
                newName: "SupervisorId");

            migrationBuilder.RenameColumn(
                name: "departement_name",
                table: "Departments",
                newName: "DepartmentName");

            migrationBuilder.RenameColumn(
                name: "departement_id",
                table: "Departments",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Charters",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "devices",
                table: "Charters",
                newName: "Devices");

            migrationBuilder.RenameColumn(
                name: "to_emp_id",
                table: "Charters",
                newName: "ToEmpId");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "Charters",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "number_of_devices",
                table: "Charters",
                newName: "CharterId");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "Charters",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "HolidayHistories",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "duration",
                table: "HolidayHistories",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "HolidayHistories",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "holiday_id",
                table: "HolidayHistories",
                newName: "HolidayId");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "HolidayHistories",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "emp_id",
                table: "HolidayHistories",
                newName: "EmpId");

            migrationBuilder.RenameColumn(
                name: "holidays_history_id",
                table: "HolidayHistories",
                newName: "HolidayHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_holidays_history_holiday_id",
                table: "HolidayHistories",
                newName: "IX_HolidayHistories_HolidayId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "OtherServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "CharterId",
                table: "Charters",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "HolidayHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Charters",
                table: "Charters",
                column: "CharterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HolidayHistories",
                table: "HolidayHistories",
                column: "HolidayHistoryId");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_HolidayHistories_Holidays_HolidayId",
                table: "HolidayHistories",
                column: "HolidayId",
                principalTable: "Holidays",
                principalColumn: "HolidayId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
