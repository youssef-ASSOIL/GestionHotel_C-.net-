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

            public List<Chambre> ListerChambresDisponibles(DateTime dateDebut, DateTime dateFin)
            {
                // Inclure les liaisons chambre-réservation
                var reservations = _context.Reservations
                    .Include(r => r.ChambreReservations) // ✅ essentiel
                    .Where(r =>
                        !r.EstAnnulee &&
                        r.DateDebut < dateFin &&
                        r.DateFin > dateDebut)
                    .ToList();

                var chambresReservees = reservations
                    .SelectMany(r => r.ChambreReservations)
                    .Select(cr => cr.ChambreId)
                    .Distinct()
                    .ToList();

                return _context.Chambres
                    .Where(c => !chambresReservees.Contains(c.Id))
                    .ToList();
            }

            public List<Chambre> GetChambresNonOccupees()
            {
                return _context.Chambres.Where(c => !c.EstOccupee).ToList();
            }
            public bool AnnulerReservation(int reservationId)
            {
                var reservation = _context.Reservations.FirstOrDefault(r => r.Id == reservationId);
                if (reservation == null) return false;

                var heuresAvant = (reservation.DateDebut - DateTime.Now).TotalHours;
                if (heuresAvant < 48)
                {
                    // Trop tard pour annuler
                    return false;
                }

                _context.Reservations.Remove(reservation);
                _context.SaveChanges();
                return true;
            }
            public bool EnregistrerArriveeClient(int reservationId)
            {
                var reservation = _context.Reservations
                    .Include(r => r.ChambreReservations)
                    .FirstOrDefault(r => r.Id == reservationId && !r.EstAnnulee);

                if (reservation == null || reservation.DateDebut.Date != DateTime.Now.Date)
                    return false; // Réservation invalide ou pas pour aujourd'hui

                // Marquer les chambres comme occupées
                foreach (var cr in reservation.ChambreReservations)
                {
                    var chambre = _context.Chambres.FirstOrDefault(c => c.Id == cr.ChambreId);
                    if (chambre != null)
                    {
                        chambre.EstOccupee = true;
                    }
                }

                // Paiement manquant ?
                if (reservation.MontantTotal <= 0)
                {
                    // TODO : Log ou alerte sur paiement non effectué
                    Console.WriteLine($"⚠️ Paiement manquant pour réservation {reservation.Id}");
                }

                _context.SaveChanges();
                return true;
            }

            public bool EnregistrerDepartClient(int reservationId)
            {
                var reservation = _context.Reservations
                    .Include(r => r.ChambreReservations)
                    .FirstOrDefault(r => r.Id == reservationId && !r.EstAnnulee);

                if (reservation == null || reservation.DateFin.Date != DateTime.Now.Date)
                    return false; // Réservation invalide ou pas pour aujourd'hui

                // Marquer les chambres comme non occupées et à nettoyer
                foreach (var cr in reservation.ChambreReservations)
                {
                    var chambre = _context.Chambres.FirstOrDefault(c => c.Id == cr.ChambreId);
                    if (chambre != null)
                    {
                        chambre.EstOccupee = false;
                        chambre.EstNettoyee = false;
                        chambre.PrioriteNettoyage = PrioriteNettoyage.Normale;
                    }
                }

                // Vérifier paiement final (ex: si le montant est toujours 0)
                if (reservation.MontantTotal <= 0)
                {
                    // TODO : log ou alerte de paiement manquant
                    Console.WriteLine($"⚠️ Paiement à régulariser pour la réservation {reservation.Id}");
                }

                _context.SaveChanges();
                return true;
            }

            public void EnvoyerEmailAvis(Client client)
            {
                var email = client.Email;
                var contenu = "Merci pour votre séjour ! Donnez-nous votre avis ici : [lien]";
                Console.WriteLine($"📧 Email envoyé à {email} : {contenu}");

                // TODO : Intégrer SendGrid, SMTP ou autre si nécessaire
            }
    
            public bool ReserverChambre(Client client, int idChambre, DateTime date, int nombreDePersonne)
            {
                var chambre = _context.Chambres
                    .FirstOrDefault(c => c.Id == idChambre &&
                                         c.Capacite >= nombreDePersonne &&
                                         !c.EstOccupee);
                if (chambre == null)
                    return false;

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

}