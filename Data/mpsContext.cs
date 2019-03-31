using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using portal.mps.Models;
namespace portal.mps.Data
{
    public class mpsContext : IdentityDbContext<mpsUser>
    {
        public mpsContext(DbContextOptions options)
            : base(options)
        {

        }
        public DbSet<Slab> Slabs { get; set; }
        public DbSet<StudentSlab> StudentSlabs { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentFee> StudentFees { get; set; }
        
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<StaffPayment> StaffPayments { get; set; }
        public DbSet<StaffSalary> StaffSalarys { get; set; }
        public DbSet<Deduction> Deductions { get; set; }

        public DbSet<ExpenseHeader> ExpenseHeaders { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ImgDoc> ImgDocs { get; set; }
        public DbSet<Bill> Bills { get; set; }
        
        public DbSet<AcademicEntity> AcademicEntities { get; set; }
        public DbSet<StudentMatrix> StudentMatrices { get; set; }
        public DbSet<SubjectSort> SubjectSort { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                {
                    relationship.DeleteBehavior = DeleteBehavior.Restrict;
                }
            builder.Entity<Slab>()
                .Property(s => s.Id)
                .UseSqlServerIdentityColumn();
        }
    }
}