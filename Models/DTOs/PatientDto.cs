namespace WebApplication5.Models.DTOs;

public class PatientDto
{
    public int? IdPatient { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = null!;
}