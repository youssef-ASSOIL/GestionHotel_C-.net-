using Archi_applicatives_MSAFE.msafe.com.business;
using Archi_applicatives_MSAFE.msafe.com.models;
using Microsoft.AspNetCore.Mvc;

namespace Archi_applicatives_MSAFE.msafe.com.controllers;

[ApiController]
[Route("api/[controller]")]
public class MenageController : ControllerBase
{
    private readonly PersonnelleMenageServicesInterface _menageService;

    public MenageController(PersonnelleMenageServicesInterface menageService)
    {
        _menageService = menageService;
    }

    // GET: api/menage/chambres-a-nettoyer
	[HttpGet("chambres-a-nettoyer")]
	public ActionResult GetChambresANettoyer()
	{
    	var chambres = _menageService.ListChambreaNettoyer();

    	var result = chambres.Select(c => new {
        	c.Numero,
        	PrioriteNettoyage = (int)c.PrioriteNettoyage,
        	c.EstNettoyee,
        	c.EstOccupee
    	});

    	return Ok(result);
	}


    // POST: api/menage/marquer-nettoyee/5
    [HttpPost("marquer-nettoyee/{id}")]
    public ActionResult<Chambre> MarquerCommeNettoyee(int id)
    {
        try
        {
            var chambre = _menageService.chambreAnettoyer(id);
            return Ok(chambre);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}