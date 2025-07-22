using Archi_applicatives_MSAFE.msafe.com.models;

namespace Archi_applicatives_MSAFE.msafe.com.business;

public interface ReceptionisteServicesInterface
{
    /*
     *
     * Réceptionniste
       
       Mêmes fonctionnalités que le client, avec plus d'informations ou de droits:
       Liste des chambre disponibles : Obtenir la liste des chambres disponibles à une plage de dates donnée, avec une information sur l'état général de la chambre (Neuf, Refaite, A refaire, Rien a signaler, Gros dégats).
       Annulation de réservation : Annuler une réservation pour un client, si l'annulation à lieu moins de 48 heures avant la date de réservation, la receptionniste peut choisir de rembourser ou non le client, malgè la règle de base.
       Gestion de l'arrivée : Noter l'occupation de la chambre et gérer les paiements non effectués.
       Gestion du départ : Marquer la chambre pour nettoyage et gérer les paiements restants.
       Envoi d'email post-séjour : Optionel - Envoyer un email type "donnez votre avis" après le départ du client.
     * 
     */
    List<Chambre> listChambres(DateTime date);
    bool ReserverChambre(Client client, TypeChambre typeChambre ,DateTime date,int numerodechambre,int numerodePersonne);
    bool DeleteReservation(Client client ,int idReservation);
    double listTrarif();
    List<Chambre> listChambresDisponibles(DateTime date);
    Chambre DeclarerChambreanettoyer(int idChambre);
    
}