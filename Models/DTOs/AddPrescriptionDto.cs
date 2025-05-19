namespace WebApplication5.Models.DTOs;

public class AddPrescriptionDto
{
    public PatientDto Patient { get; set; } = null!;
    public List<PrescriptionMedicamentDto> Medicaments { get; set; } = null!;
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public int IdDoctor { get; set; }
}