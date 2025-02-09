using CharityProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;

namespace CharityProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<charter> charter { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Devices> Devices { get; set; }
        public DbSet<employee> employee { get; set; }
        public DbSet<employee_details> employee_details { get; set; }
        public DbSet<ExternalTransaction> ExternalTransactions { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<HolidayHistory> HolidayHistories { get; set; }
        public DbSet<letter> letters { get; set; }
        public DbSet<salaries_history> SalaryHistories { get; set; }
        public DbSet<Salary> EmployeeSalary { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<OtherService> OtherServices { get; set; }
        public DbSet<Referral> Referrals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //  For new database table names:
            //modelBuilder.Entity<charter>().ToTable("charter");
            //modelBuilder.Entity<Department>().ToTable("department");
            //modelBuilder.Entity<Devices>().ToTable("devices");
            //modelBuilder.Entity<employee>().ToTable("employee");
            //modelBuilder.Entity<employee_details>().ToTable("employee_details");
            //modelBuilder.Entity<ExternalTransaction>().ToTable("external_transactions");
            //modelBuilder.Entity<Holiday>().ToTable("holidays");
            //modelBuilder.Entity<HolidayHistory>().ToTable("holidays_history");
            //modelBuilder.Entity<letter>().ToTable("letters");
            //modelBuilder.Entity<salaries_history>().ToTable("salaries_history");
            //modelBuilder.Entity<Salary>().ToTable("employee_salary");
            //modelBuilder.Entity<Transaction>().ToTable("transactions");
            //modelBuilder.Entity<OtherService>().ToTable("other_services");
            //modelBuilder.Entity<Referral>().ToTable("referrals");
        }

    }
}
