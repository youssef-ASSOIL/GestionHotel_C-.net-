using Archi_applicatives_MSAFE.Data;
using Archi_applicatives_MSAFE.msafe.com.models;

namespace Archi_applicatives_MSAFE.msafe.com.business;

public class ReceptionisteServicesImplementation:ReceptionisteServicesInterface
{
        private readonly HotelDbContext _context;

        public ReceptionisteServicesImplementation(HotelDbContext context)
        {
            _context = context;
        }

        public List<Chambre> listChambres(DateTime date)
        {
            return _context.Chambres.ToList();
        }

        
        public bool ReserverChambre(Client client, TypeChambre typeChambre, DateTime date, int numerodechambre, int numerodePersonne)
        {
            // 1. Trouver la chambre disponible avec les critères
            var chambre = _context.Chambres
                .FirstOrDefault(c => c.Numero == numerodechambre.ToString()
                                     && c.Type == typeChambre
                                     && !c.EstOccupee
                                     && c.Capacite >= numerodePersonne);

            if (chambre == null)
                return false;

            // 2. Vérifier ou ajouter le client
            var existingClient = _context.Clients.FirstOrDefault(c => c.Id == client.Id);
            if (existingClient == null)
            {
                _context.Clients.Add(client);
                _context.SaveChanges();
                existingClient = client;
            }

            // 3. Créer la réservation
            var reservation = new Reservation
            {
                ClientId = existingClient.Id,
                Client = existingClient,
                DateDebut = date,
                DateFin = date.AddDays(1),
                MontantTotal = chambre.TarifNuit,
                EstAnnulee = false
            };
            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            // 4. Créer la liaison chambre ↔ réservation
            var lien = new ChambreReservation
            {
                ChambreId = chambre.Id,
                ReservationId = reservation.Id
            };
            _context.ChambreReservations.Add(lien);

            // 5. Marquer la chambre comme occupée
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

            public List<Chambre> listChambresDisponibles(DateTime date)
            {
                throw new NotImplementedException();
            }

            public Chambre DeclarerChambreanettoyer(int idChambre)
            {
                var chambre = _context.Chambres.Find(idChambre);
                if (chambre == null) return null;

                chambre.EstNettoyee = true;
                _context.SaveChanges();

                return chambre;
            }

            public bool SupprimerChambre(int id)
            {
                var chambre = _context.Chambres.Find(id);
                if (chambre == null) return false;

                _context.Chambres.Remove(chambre);
                _context.SaveChanges();

                return true;
            }

        
}