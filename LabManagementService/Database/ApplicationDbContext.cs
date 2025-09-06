using LabManagementService.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabManagementService.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<LabOrder> LabOrders { get; set; }
        public DbSet<LabTest> LabTests { get; set; }
        public DbSet<LabResult> LabResults { get; set; }
        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LabOrder>()
                .ToTable("LabOrders");

            modelBuilder.Entity<LabOrder>()
                .Property(p => p.LabOrderId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<LabOrder>()
                .HasKey(p => p.LabOrderId);

            modelBuilder.Entity<LabOrder>()
                .Property(p => p.PatientId)
                .IsRequired();

            modelBuilder.Entity<LabOrder>()
                .Property(p => p.OrderDate)
                .IsRequired();

            modelBuilder.Entity<LabOrder>()
                .Property(p => p.Status)
                .IsRequired();

            modelBuilder.Entity<LabOrder>()
                .Property(p => p.OrderedBy)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<LabOrder>()
                .HasOne(p => p.LabResult)
                .WithOne(p => p.LabOrder)
                .HasForeignKey<LabResult>(p => p.LabOrderId)
                .IsRequired(false);



            modelBuilder.Entity<LabTest>()
                .ToTable("LabTests");

            modelBuilder.Entity<LabTest>()
                .Property(p => p.LabTestId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<LabTest>()
                .HasKey(p => p.LabTestId);

            modelBuilder.Entity<LabTest>()
                .Property(p => p.TestName)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<LabTest>()
                .Property(p => p.ReferenceRange)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<LabTest>()
                .HasMany(p => p.LabOrders)
                .WithOne(p => p.LabTest)
                .HasForeignKey(p => p.LabTestId)
                .IsRequired();


            modelBuilder.Entity<LabResult>()
                .ToTable("LabResults");

            modelBuilder.Entity<LabResult>()
                .HasKey(p => p.LabResultId);

            modelBuilder.Entity<LabResult>()
                .Property(p => p.Value)
                .HasMaxLength(100)
                .IsRequired();


            modelBuilder.Entity<Patient>()
                .ToTable("Patients");

            modelBuilder.Entity<Patient>()
                .Property(p => p.Id)
                .ValueGeneratedNever();
        }
    }
}
