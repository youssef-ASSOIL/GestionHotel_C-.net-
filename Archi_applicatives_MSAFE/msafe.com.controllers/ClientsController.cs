using Archi_applicatives_MSAFE.msafe.com.business;
using Archi_applicatives_MSAFE.msafe.com.models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Archi_applicatives_MSAFE.msafe.com.controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly ClientsServicesInterface _reservationService;

    public ClientsController(ClientsServicesInterface reservationService)
    {
        _reservationService = reservationService;
    }

    // GET: api/reservation/chambres-disponibles?dateDebut=2025-07-20&dateFin=2025-07-22&personnes=2
    [HttpGet("chambres-disponibles")]
    public ActionResult<List<Chambre>> GetChambresDisponibles([FromQuery] DateTime dateDebut, [FromQuery] DateTime dateFin, [FromQuery] int personnes)
    {
        try
        {
            var chambres = _reservationService.ListeChambresDisponibles(dateDebut, dateFin, personnes);
            return Ok(chambres);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST: api/reservation/reserver
    [HttpPost("reserver")]
    public ActionResult<Reservation> Reserver([FromBody] ReserverRequest request)
    {
        try
        {
            var reservation = _reservationService.ReserverChambres(
                request.ClientId,
                request.DateDebut,
                request.DateFin,
                request.ChambreIds,
                request.NumeroCarteBancaire
            );
            return Ok(reservation);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST: api/reservation/annuler/5
    [HttpPost("annuler/{id}")]
    public ActionResult<string> AnnulerReservation(int id)
    {
        try
        {
            bool remboursement = _reservationService.AnnulerReservation(id);
            if (remboursement)
                return Ok("Réservation annulée avec remboursement.");
            else
                return Ok("Réservation annulée sans remboursement (moins de 48h avant).");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}

// DTO pour la requête de réservation
	public class ReserverRequest
{
    public int ClientId { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public List<int> ChambreIds { get; set; }
    public string NumeroCarteBancaire { get; set; }
}
