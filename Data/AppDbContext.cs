using Microsoft.EntityFrameworkCore;
using WebApplication5.Models;

namespace WebApplication5.Data;

public class AppDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set;}
    public DbSet<Medicament> Medicaments { get; set;}
    public DbSet<Prescription> Prescriptions { get; set;}
    public DbSet<Patient> Patients { get; set;}
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set;}



    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Prescription_Medicament: klucz złożony
        modelBuilder.Entity<PrescriptionMedicament>()
            .HasKey(pm => new { pm.IdPrescription, pm.IdMedicament });

        // Relacja Prescription_Medicament -> Prescription (wiele do jednego)
        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Prescription)
            .WithMany(p => p.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdPrescription);

        // Relacja Prescription_Medicament -> Medicament (wiele do jednego)
        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Medicament)
            .WithMany(m => m.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdMedicament);

        // Relacja Prescription -> Patient (wiele do jednego)
        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.Patient)
            .WithMany(pa => pa.Prescriptions)
            .HasForeignKey(p => p.IdPatient);

        // Relacja Prescription -> Doctor (wiele do jednego)
        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.Doctor)
            .WithMany(d => d.Prescriptions)
            .HasForeignKey(p => p.IdDoctor);
    }

    
    
    
    
}