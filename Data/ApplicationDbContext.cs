using CharityProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CharityProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties for your entities
        public DbSet<Charter> Charters { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }
        public DbSet<ExternalTransaction> ExternalTransactions { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<HolidayHistory> HolidayHistories { get; set; }
        public DbSet<Letter> Letters { get; set; }
        public DbSet<SalaryHistory> SalaryHistories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<OtherService> OtherServices { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Additional configurations if needed
        }
    }
}
