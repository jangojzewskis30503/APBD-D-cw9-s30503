namespace WebApplication5.Models.DTOs;

public class PrescriptionMedicamentDto
{
    public int IdMedicament { get; set; }
    
    public int? Dose { get; set; }
    public string Description { get; set; } = null!;
        public string? MedicamentName { get; set; }
}