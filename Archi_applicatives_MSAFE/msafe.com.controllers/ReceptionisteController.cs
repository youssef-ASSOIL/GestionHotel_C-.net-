using Archi_applicatives_MSAFE.Data;
using Microsoft.AspNetCore.Mvc;
using Archi_applicatives_MSAFE.msafe.com.business;
using Archi_applicatives_MSAFE.msafe.com.models;

namespace Archi_applicatives_MSAFE.msafe.com.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceptionisteController : ControllerBase
    {
        /*
         *
         * Mêmes fonctionnalités que le client, avec plus d'informations ou de droits:
           Liste des chambre disponibles : Obtenir la liste des chambres disponibles à une plage de dates donnée, avec une information sur l'état général de la chambre (Neuf, Refaite, A refaire, Rien a signaler, Gros dégats).
           Annulation de réservation : Annuler une réservation pour un client, si l'annulation à lieu moins de 48 heures avant la date de réservation, la receptionniste peut choisir de rembourser ou non le client, malgè la règle de base.
           Gestion de l'arrivée : Noter l'occupation de la chambre et gérer les paiements non effectués.
           Gestion du départ : Marquer la chambre pour nettoyage et gérer les paiements restants.
           Envoi d'email post-séjour : Optionel - Envoyer un email type "donnez votre avis" après le départ du client.
         */
        private readonly ReceptionisteServicesInterface _service;
        private readonly HotelDbContext _context;
        
        public ReceptionisteController(ReceptionisteServicesInterface service)
        {
            _service = service;
        }
        
        [HttpGet("non-occupees")]
        public IActionResult GetChambresNonOccupees()
        {
            try
            {
                var chambres = _service.GetChambresNonOccupees();
                return Ok(chambres);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR: " + ex.Message); // ou Logger.LogError(ex, ...)
                return StatusCode(500, "Erreur serveur: " + ex.Message);
            }
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

        [HttpGet("disponibles")]
        public IActionResult GetChambresDisponibles([FromQuery] DateTime dateDebut, [FromQuery] DateTime dateFin)
        {
            try
            {
                var chambres = _service.ListerChambresDisponibles(dateDebut, dateFin);
                return Ok(chambres);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR : " + ex.Message); // log console
                return StatusCode(500, "Erreur interne : " + ex.Message); // Swagger/curl
            }
        }
        
        [HttpDelete("annuler/{reservationId}")]
        public IActionResult AnnulerReservation(int reservationId)
        {
            var ok = _service.AnnulerReservation(reservationId);
            if (!ok) return BadRequest("Annulation refusée (moins de 48h ou réservation introuvable)");
            return Ok("Réservation annulée");
        }

        [HttpPost("arrivee/{reservationId}")]
        public IActionResult EnregistrerArrivee(int reservationId)
        {
            var ok = _service.EnregistrerArriveeClient(reservationId);
            if (!ok) return BadRequest("Arrivée non enregistrée (réservation invalide ou déjà annulée)");
            return Ok("Arrivée enregistrée");
        }
    
        [HttpPost("depart/{reservationId}")]
        public IActionResult EnregistrerDepart(int reservationId)
        {
            var ok = _service.EnregistrerArriveeClient(reservationId);
            if (!ok) return BadRequest("Départ non enregistré (réservation invalide ou hors date)");
            return Ok("Départ enregistré, chambre marquée pour nettoyage");
        }
        
        [HttpPost("envoyer-avis/{clientId}")]
        public IActionResult EnvoyerEmailAvis(int clientId)
        {
            var client = _context.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client == null)
                return NotFound("Client introuvable");

            _service.EnvoyerEmailAvis(client);
            return Ok("Email d’avis envoyé (simulation)");
        }


    }
}