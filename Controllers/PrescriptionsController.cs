using Microsoft.AspNetCore.Mvc;

using WebApplication5.Models.DTOs;
using WebApplication5.Services;

namespace WebApplication5.Controllers;

 




[ApiController]
[Route("api/[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _service;

    public PrescriptionsController(IPrescriptionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] AddPrescriptionDto prescriptionData)
    {
        try
        {
            var prescriptionId = await _service.AddPrescriptionAsync(prescriptionData);
            return CreatedAtAction(nameof(GetPrescriptionDetails), new { id = prescriptionId }, null);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Wystąpił błąd podczas dodawania recepty: " + e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPrescriptionDetails(int id)
    {
        try
        {
            var prescription = await _service.GetPrescriptionDetailsAsync(id);
            return prescription == null ? NotFound() : Ok(prescription);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Wystąpił błąd podczas pobierania recepty: " + e.Message);
        }
    }
}