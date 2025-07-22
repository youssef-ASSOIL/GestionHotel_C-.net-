using Archi_applicatives_MSAFE.msafe.com.models;

namespace Archi_applicatives_MSAFE.msafe.com.business;

public interface PersonnelleMenageServicesInterface
{
    /*
     *
     * Personnel de Ménage
       
       Liste des chambres à nettoyer : Accéder à la liste des chambres à nettoyer, avec priorisation (une chambre déjà nettoyé et non occupée depuis n'est pas à nettoyer).
       Marquage des chambres nettoyées : Noter une chambre comme nettoyée.
       Notification de casse : Optionel - Signaler des dommages pour ajustement des frais de paiement.
     */
    
    List<Chambre> ListChambreaNettoyer();
    Chambre chambreAnettoyer();
    

}