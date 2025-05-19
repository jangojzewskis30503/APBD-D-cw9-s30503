using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication5.Models;

[Table("Prescription_Medicaments")]
[PrimaryKey(nameof(IdMedicament), nameof(IdPrescription))]
public class PrescriptionMedicament
{
    [ForeignKey("Medicament")] public int IdMedicament { get; set; }
    [ForeignKey("IdPrescription")] public int IdPrescription { get; set; }
    public int? Dose { get; set; }
    [MaxLength(100)] public string Details { get; set; } = null!;
    
    public virtual Medicament Medicament { get; set; } = null!;
    public virtual Prescription Prescription { get; set; } = null!;
}