using Archi_applicatives_MSAFE.msafe.com.models;
using System;
using System.Collections.Generic;

namespace Archi_applicatives_MSAFE.msafe.com.business;

public interface ClientsServicesInterface
{
    // Obtenir la liste des chambres disponibles sur une plage de dates
    List<Chambre> ListeChambresDisponibles(DateTime dateDebut, DateTime dateFin, int nombrePersonnes);

    // Réserver une ou plusieurs chambres
    Reservation ReserverChambres(int clientId, DateTime dateDebut, DateTime dateFin, List<int> chambreIds, string numeroCarteBancaire);

    // Annuler une réservation (avec remboursement si applicable)
    bool AnnulerReservation(int reservationId);
}