using Archi_applicatives_MSAFE.Data;
using Archi_applicatives_MSAFE.msafe.com.models;
using Microsoft.EntityFrameworkCore;

namespace Archi_applicatives_MSAFE.msafe.com.business;

public class PersonnelleMenageServicesImplementation : PersonnelleMenageServicesInterface
{
    private readonly HotelDbContext _context;

    public PersonnelleMenageServicesImplementation(HotelDbContext context)
    {
        _context = context;
    }

    // Liste des chambres à nettoyer
    public List<Chambre> ListChambreaNettoyer()
    {
        return _context.Chambres
            .Where(c => !c.EstNettoyee && !c.EstOccupee)
            .OrderBy(c => c.PrioriteNettoyage)
            .ToList();
    }

    // Marquer une chambre comme nettoyée (ex: appeler avec l'ID)
    public Chambre chambreAnettoyer(int chambreId)
    {
        var chambre = _context.Chambres.FirstOrDefault(c => c.Id == chambreId);
        if (chambre == null)
            throw new Exception($"Chambre {chambreId} non trouvée");

        chambre.EstNettoyee = true;
        _context.SaveChanges();
        return chambre;
    }
}