using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication5.Models;


[Table("Medicament")]
public class Medicament
{
    [Key] public int IdMedicament { get; set; }
    [MaxLength(100)] public string Name { get; set; } = null!;
    [MaxLength(100)] public string Descritpion { get; set; } = null!;
    [MaxLength(100)] public string Type { get; set; } = null!;
    
    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; } = new List<PrescriptionMedicament>();
    
}