using Archi_applicatives_MSAFE.Data;
using Archi_applicatives_MSAFE.msafe.com.models;

namespace Archi_applicatives_MSAFE.msafe.com.business;

public class ClientsServicesImplementation : ClientsServicesInterface
{
    private readonly HotelDbContext _context;

    public ClientsServicesImplementation(HotelDbContext context)
    {
        _context = context;
    }
    public bool ReserverChambre(Client client, TypeChambre typeChambre, DateTime date, int numerodechambre, int numerodePersonne)
    {
        var chambre = _context.Chambres.FirstOrDefault(c =>
            c.Id == numerodechambre &&
            c.Type == typeChambre &&
            c.Capacite >= numerodePersonne &&
            !c.EstOccupee);

        if (chambre == null)
            return false;

        var existingClient = _context.Clients.FirstOrDefault(c => c.Id == client.Id);
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
            EstAnnulee = false
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


    public bool DeleteReservation(Client client, int idReservation)
    {
        throw new NotImplementedException();
    }

    public double listTrarif()
    {
        throw new NotImplementedException();
    }

    public List<Chambre> listChambres(DateTime date)
    {
        // Étape 1 : Récupérer les réservations actives à cette date
        var reservationsCeJour = _context.Reservations
            .Where(r => !r.EstAnnulee &&
                        r.DateDebut <= date &&
                        r.DateFin >= date)
            .SelectMany(r => r.ChambreReservations)
            .Select(cr => cr.ChambreId)
            .Distinct()
            .ToList();

        // Étape 2 : Récupérer les chambres non concernées
        var chambresDisponibles = _context.Chambres
            .Where(c => !reservationsCeJour.Contains(c.Id))
            .ToList();

        return chambresDisponibles;
    }

}