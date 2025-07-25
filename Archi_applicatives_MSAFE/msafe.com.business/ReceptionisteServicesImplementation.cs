using Archi_applicatives_MSAFE.Data;
using Archi_applicatives_MSAFE.msafe.com.models;
using Microsoft.EntityFrameworkCore;

namespace Archi_applicatives_MSAFE.msafe.com.business;

public class ReceptionisteServicesImplementation:ReceptionisteServicesInterface
{
        private readonly HotelDbContext _context;

    public ReceptionisteServicesImplementation(HotelDbContext context)
    {
        _context = context;
    }

    public List<Chambre> ListerChambresDisponibles(DateTime dateDebut, DateTime dateFin)
    {
        var chambresReservees = _context.Reservations
            .Where(r => !r.EstAnnulee &&
                        r.DateDebut < dateFin &&
                        r.DateFin > dateDebut)
            .SelectMany(r => r.ChambreReservations)
            .Select(cr => cr.ChambreId)
            .Distinct()
            .ToList();

        var chambresDisponibles = _context.Chambres
            .Where(c => !chambresReservees.Contains(c.Id))
            .ToList();

        return chambresDisponibles;
    }

    public bool ReserverChambre(Client client, int idChambre, DateTime date, int nombreDePersonne)
    {
        var chambre = _context.Chambres.FirstOrDefault(c => c.Id == idChambre && !c.EstOccupee && c.Capacite >= nombreDePersonne);
        if (chambre == null) return false;

        var existingClient = _context.Clients.FirstOrDefault(c => c.Email == client.Email);
        if (existingClient == null)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
            existingClient = client;
        }

        var reservation = new Reservation
        {
            ClientId = existingClient.Id,
            DateDebut = date,
            DateFin = date.AddDays(1),
            MontantTotal = chambre.TarifNuit,
            EstAnnulee = false,
            DateCreation = DateTime.Now
        };

        _context.Reservations.Add(reservation);
        _context.SaveChanges();

        _context.ChambreReservations.Add(new ChambreReservation
        {
            ChambreId = chambre.Id,
            ReservationId = reservation.Id
        });

        chambre.EstOccupee = true;
        _context.SaveChanges();
        return true;
    }

    public bool AnnulerReservation(int reservationId, bool remboursementForce = false)
    {
        var reservation = _context.Reservations.Include(r => r.ChambreReservations).ThenInclude(cr => cr.Chambre)
            .FirstOrDefault(r => r.Id == reservationId);
        if (reservation == null || reservation.EstAnnulee) return false;

        var heuresAvant = (reservation.DateDebut - DateTime.Now).TotalHours;
        if (heuresAvant < 48 && !remboursementForce) return false;

        reservation.EstAnnulee = true;
        foreach (var cr in reservation.ChambreReservations)
            cr.Chambre.EstOccupee = false;

        _context.SaveChanges();
        return true;
    }

    public bool EnregistrerArriveeClient(int reservationId)
    {
        var reservation = _context.Reservations.Include(r => r.ChambreReservations).ThenInclude(cr => cr.Chambre)
            .FirstOrDefault(r => r.Id == reservationId && !r.EstAnnulee);
        if (reservation == null) return false;

        foreach (var cr in reservation.ChambreReservations)
            cr.Chambre.EstOccupee = true;

        _context.SaveChanges();
        return true;
    }

    public bool EnregistrerDepartClient(int reservationId)
    {
        var reservation = _context.Reservations.Include(r => r.ChambreReservations).ThenInclude(cr => cr.Chambre)
            .FirstOrDefault(r => r.Id == reservationId && !r.EstAnnulee);
        if (reservation == null) return false;

        foreach (var cr in reservation.ChambreReservations)
        {
            cr.Chambre.EstOccupee = false;
            cr.Chambre.PrioriteNettoyage = PrioriteNettoyage.Haute;
        }

        _context.SaveChanges();
        return true;
    }

    public void EnvoyerEmailAvis(Client client)
    {
        // Simulation d'envoi d'email
        Console.WriteLine($"Email envoyé à {client.Email} : Merci pour votre séjour ! Laissez-nous un avis.");
    }

    public Chambre DeclarerChambreanettoyer(int idChambre)
    {
        var chambre = _context.Chambres.FirstOrDefault(c => c.Id == idChambre);
        if (chambre == null) return null;
        chambre.PrioriteNettoyage = PrioriteNettoyage.Haute;
        _context.SaveChanges();
        return chambre;
    }

    public List<Chambre> GetChambresNonOccupees()
    {
        return _context.Chambres.Where(c => !c.EstOccupee).ToList();
    }

}