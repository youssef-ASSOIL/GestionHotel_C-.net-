using Archi_applicatives_MSAFE.msafe.com.models;

namespace Archi_applicatives_MSAFE.msafe.com.business;

public interface ClientsServicesInterface
{
    bool ReserverChambre(Client client, TypeChambre typeChambre ,DateTime date,int numerodechambre,int numerodePersonne);
    bool DeleteReservation(Client client ,int idReservation);
    double listTrarif();
    List<Chambre> listChambres(DateTime date);

    
}