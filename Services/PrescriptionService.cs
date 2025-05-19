using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Models;
using WebApplication5.Models.DTOs;

namespace WebApplication5.Services;

public interface IPrescriptionService
{
    Task<int> AddPrescriptionAsync(AddPrescriptionDto request);
    Task<PatientDetailsDto> GetPatientDetailsAsync(int id);
    Task<PrescriptionDetailsDto?> GetPrescriptionDetailsAsync(int id);
}

public class PrescriptionService : IPrescriptionService
{
    private readonly AppDbContext _context;

    public PrescriptionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddPrescriptionAsync(AddPrescriptionDto request)
    {
        // Walidacja 1: Maksymalnie 10 leków na receptę
        if (request.Medicaments.Count > 10)
            throw new ArgumentException("Recepta może zawierać maksymalnie 10 leków");

       
        if (request.DueDate < request.Date)
            throw new ArgumentException(
                "Data ważności (DueDate) nie może być wcześniejsza niż data wystawienia (Date)");

        // Obsługa pacjenta - aktualizacja istniejącego lub dodanie nowego
        Patient patient = await HandlePatientAsync(request.Patient);

        // Walidacja 3: Sprawdzenie czy wszystkie leki istnieją w bazie
        await ValidateMedicamentsExistAsync(request.Medicaments);

        // Utworzenie nowej recepty
        var prescription = new Prescription
        {
            Date = request.Date,
            DueDate = request.DueDate,
            IdPatient = patient.IdPatient,
            IdDoctor = request.IdDoctor, 
            PrescriptionMedicaments = request.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Details = m.Description
            }).ToList()
        };

        // Dodanie recepty do bazy i zapis zmian
        await _context.Prescriptions.AddAsync(prescription);
        await _context.SaveChangesAsync();

        return prescription.IdPrescription;
    }

    public async Task<PatientDetailsDto> GetPatientDetailsAsync(int id)
    {
        // Pobranie pacjenta z wszystkimi powiązanymi danymi (eager loading)
        var patient = await _context.Patients
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.Doctor)
            .FirstOrDefaultAsync(p => p.IdPatient == id);

        // Jeśli pacjent nie istnieje, zwraca null
        if (patient == null)
            return null;

        // Mapowanie danych pacjenta na DTO
        return new PatientDetailsDto
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.BirthDate,
            Email = patient.Email,
            // Mapowanie recept z sortowaniem według DueDate
            Prescriptions = patient.Prescriptions
                .OrderBy(pr => pr.DueDate)
                .Select(pr => new PrescriptionDto
                {
                    IdPrescription = pr.IdPrescription,
                    Date = pr.Date,
                    DueDate = pr.DueDate,
                    // Mapowanie leków dla danej recepty
                    Medicaments = pr.PrescriptionMedicaments.Select(pm => new PrescriptionMedicamentDto
                    {
                        IdMedicament = pm.IdMedicament,
                        MedicamentName = pm.Medicament.Name,
                        Dose = pm.Dose,
                        Description = pm.Details
                    }).ToList(),
                    // Mapowanie danych lekarza
                    Doctor = new DoctorDto
                    {
                        IdDoctor = pr.Doctor.IdDoctor,
                        FirstName = pr.Doctor.FirstName,
                        LastName = pr.Doctor.LastName
                    }
                }).ToList()
        };
    }

    public async Task<PrescriptionDetailsDto> GetPrescriptionDetailsAsync(int id)
    {
        var prescription = await _context.Prescriptions
            .Include(p => p.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .Include(p => p.Doctor)
            .FirstOrDefaultAsync(p => p.IdPrescription == id);

        return prescription == null ? null : MapToPrescriptionDetailsDto(prescription);
    }


    private PrescriptionDetailsDto MapToPrescriptionDetailsDto(Prescription prescription)
    {
        return new PrescriptionDetailsDto
        {
            IdPrescription = prescription.IdPrescription,
            Date = prescription.Date,
            DueDate = prescription.DueDate,
            Medicaments = prescription.PrescriptionMedicaments.Select(pm => new PrescriptionMedicamentDto
            {
                IdMedicament = pm.IdMedicament,
               
                Dose = pm.Dose,
                Description = pm.Details
            }).ToList(),
            Doctor = new DoctorDto
            {
                IdDoctor = prescription.Doctor.IdDoctor,
                FirstName = prescription.Doctor.FirstName,
                LastName = prescription.Doctor.LastName
            }
        };
    }


    private async Task<Patient> HandlePatientAsync(PatientDto patientDto)
    {
        // Jeśli podano ID pacjenta, próbuje go znaleźć
        if (patientDto.IdPatient.HasValue)
        {
            var existingPatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdPatient == patientDto.IdPatient.Value);

            if (existingPatient != null)
                return existingPatient;
        }

        // Jeśli nie znaleziono pacjenta lub nie podano ID, tworzy nowego
        var newPatient = new Patient
        {
            FirstName = patientDto.FirstName,
            LastName = patientDto.LastName,
            BirthDate = patientDto.BirthDate,
            Email = patientDto.Email
        };

        await _context.Patients.AddAsync(newPatient);
        await _context.SaveChangesAsync();

        return newPatient;
    }

    private async Task ValidateMedicamentsExistAsync(List<PrescriptionMedicamentDto> medicaments)
    {
        // Pobranie ID wszystkich leków z żądania
        var medicamentIds = medicaments.Select(m => m.IdMedicament).ToList();

        // Sprawdzenie ile z tych leków istnieje w bazie
        var existingCount = await _context.Medicaments
            .Where(m => medicamentIds.Contains(m.IdMedicament))
            .CountAsync();

        // Jeśli liczba znalezionych leków jest mniejsza niż liczba ID, to znaczy że część leków nie istnieje
        if (existingCount != medicamentIds.Count)
            throw new InvalidOperationException("Jeden lub więcej leków nie istnieje w bazie danych");
    }
}