using Archi_applicatives_MSAFE.Data;
using Archi_applicatives_MSAFE.msafe.com.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Archi_applicatives_MSAFE.msafe.com.business;

public class ClientsServicesImplementation : ClientsServicesInterface
{
    private readonly HotelDbContext _context;

    public ClientsServicesImplementation(HotelDbContext context)
    {
        _context = context;
    }

    // Méthode pour obtenir les chambres disponibles selon la plage de dates et la capacité demandée
    public List<Chambre> ListeChambresDisponibles(DateTime dateDebut, DateTime dateFin, int nombrePersonnes)
    {
        // Toutes les chambres qui ont une capacité suffisante
        var chambresCapacite = _context.Chambres
            .Where(c => c.Capacite >= nombrePersonnes && !c.EstOccupee)
            .ToList();

        // Identifier les chambres déjà réservées sur la plage donnée
        var chambresOccupees = _context.ChambreReservations
            .Include(cr => cr.Reservation)
            .Where(cr => 
                (dateDebut < cr.Reservation.DateFin && dateFin > cr.Reservation.DateDebut)
                && !cr.Reservation.EstAnnulee
            )
            .Select(cr => cr.ChambreId)
            .Distinct()
            .ToList();

        // Filtrer les chambres non occupées par réservation sur la plage
        var chambresDisponibles = chambresCapacite
            .Where(c => !chambresOccupees.Contains(c.Id))
            .ToList();

        return chambresDisponibles;
    }

    // Réserver des chambres si elles sont disponibles et simuler paiement
    public Reservation ReserverChambres(int clientId, DateTime dateDebut, DateTime dateFin, List<int> chambreIds, string numeroCarteBancaire)
    {
        if (dateDebut >= dateFin)
            throw new ArgumentException("Date de début doit être avant date de fin");

        var chambres = ListeChambresDisponibles(dateDebut, dateFin, 0)
            .Where(c => chambreIds.Contains(c.Id))
            .ToList();

        if (chambres.Count != chambreIds.Count)
            throw new Exception("Certaines chambres ne sont pas disponibles sur cette plage.");

        // Calcul du montant total (tarif * nombre de nuits)
        var nbNuits = (dateFin - dateDebut).Days;
        decimal montantTotal = chambres.Sum(c => c.TarifNuit) * nbNuits;

        // Simuler le paiement (faux service)
        if (!SimulerPaiement(numeroCarteBancaire, montantTotal))
            throw new Exception("Paiement refusé");

        // Créer la réservation
        var reservation = new Reservation
        {
            ClientId = clientId,
            DateDebut = dateDebut,
            DateFin = dateFin,
            MontantTotal = montantTotal,
            EstAnnulee = false,
            DateCreation = DateTime.UtcNow,
        };

        _context.Reservations.Add(reservation);
        _context.SaveChanges();

        // Associer les chambres réservées
        foreach (var chambre in chambres)
        {
            var cr = new ChambreReservation
            {
                ChambreId = chambre.Id,
                ReservationId = reservation.Id
            };
            _context.ChambreReservations.Add(cr);
        }

        _context.SaveChanges();

        return reservation;
    }

    // Annulation de réservation avec gestion remboursement
    public bool AnnulerReservation(int reservationId)
    {
        var reservation = _context.Reservations
            .FirstOrDefault(r => r.Id == reservationId);

        if (reservation == null)
            throw new Exception("Réservation non trouvée");

        if (reservation.EstAnnulee)
            throw new Exception("Réservation déjà annulée");

        var now = DateTime.UtcNow;
        var heuresAvantDebut = (reservation.DateDebut - now).TotalHours;

        // Annulation possible, remboursement si plus de 48h avant début
        bool remboursement = heuresAvantDebut >= 48;

        // Mise à jour état réservation
        reservation.EstAnnulee = true;
        _context.SaveChanges();

        // Si remboursement => ici tu peux appeler un faux service de remboursement
        if (remboursement)
        {
            // Logique de remboursement
            // Exemple : SimulerRemboursement(reservation.MontantTotal);
        }

        return remboursement;
    }

    private bool SimulerPaiement(string numeroCarteBancaire, decimal montant)
    {
        // Faux service paiement : ici on accepte si numéro non vide et montant > 0
        return !string.IsNullOrWhiteSpace(numeroCarteBancaire) && montant > 0;
    }
}
