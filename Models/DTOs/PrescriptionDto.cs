namespace WebApplication5.Models.DTOs;

public class PrescriptionDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<PrescriptionMedicamentDto> Medicaments { get; set; } = null!;
    public DoctorDto Doctor { get; set; } = null!;
}