using Microsoft.AspNetCore.Mvc;
using Archi_applicatives_MSAFE.msafe.com.business;
using Archi_applicatives_MSAFE.msafe.com.models;

namespace Archi_applicatives_MSAFE.msafe.com.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceptionisteController : ControllerBase
    {
        private readonly ClientsServicesInterface _service;

        public ReceptionisteController(ClientsServicesInterface service)
        {
            _service = service;
        }

        [HttpGet("chambres")]
        public IActionResult GetChambres([FromQuery] DateTime? date = null)
        {
            var result = _service.listChambres(date ?? DateTime.Today);
            return base.Ok(result); // utilisation explicite de base.Ok() pour contourner le masquage
        }
        
        [HttpDelete("deletereservation/{idReservation}")]
        public IActionResult DeleteReservation(int idReservation, [FromBody] Client client)
        {
            var deleted = _service.DeleteReservation(client, idReservation);

            if (!deleted)
                return NotFound(new { message = $"Réservation avec l'id {idReservation} introuvable pour ce client." });

            return NoContent(); // 204
        }
        [HttpPost("ajouterreservation/{idChambre}")]
        public IActionResult AjouterReservation(
            [FromBody] Client client,
            int idChambre,
            [FromQuery] DateTime date,
            [FromQuery] int nombreDePersonne)
        {
            var success = _service.ReserverChambre(
                client,
                typeChambre: TypeChambre.Double, // facultatif ou récupérable selon ta logique
                date,
                idChambre,
                nombreDePersonne
            );

            if (!success)
                return BadRequest(new { message = "Impossible de réserver la chambre." });

            return Ok(new { message = "Réservation ajoutée avec succès." });
        }

        
    }
}