using Microsoft.AspNetCore.Mvc;
using WebApplication5.Services;

namespace WebApplication5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPrescriptionService _service;

    public PatientsController(IPrescriptionService service)
    {
        _service = service;
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientDetails([FromRoute] int id)
    {
        try
        {
            var patientDetails = await _service.GetPatientDetailsAsync(id);
            if (patientDetails == null) return NotFound($"no patient found with id {id}");
            return Ok(patientDetails);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Error loading patient" + e.Message);
        }
    }
    
}