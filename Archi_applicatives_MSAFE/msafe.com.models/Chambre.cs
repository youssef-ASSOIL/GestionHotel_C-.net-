using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archi_applicatives_MSAFE.msafe.com.models;
[Table("chambre")]
public class Chambre
{
    [Key]
    public int Id { get; set; }
    
    private string _numero = string.Empty;
    public string Numero
    {
        get => _numero;
        set => _numero = value ?? throw new ArgumentNullException(nameof(value));
    }

    public TypeChambre Type { get; set; }
    
    public int Capacite { get; set; } 
    
    public decimal TarifNuit { get; set; }
    
    public EtatChambre Etat { get; set; }
    
    public PrioriteNettoyage PrioriteNettoyage { get; set; }
    
    public bool EstNettoyee { get; set; }
    
    public bool EstOccupee { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public List<ChambreReservation> ChambreReservations { get; set; } = new();

    public Chambre() { }

    public Chambre(int id, string numero, TypeChambre type, int capacite, decimal tarif)
    {
        Id = id;
        Numero = numero;
        Type = type;
        Capacite = capacite;
        TarifNuit = tarif;
        Etat = EtatChambre.RienÀSignaler;
        PrioriteNettoyage = PrioriteNettoyage.Aucune;
        EstNettoyee = true;
        EstOccupee = false;
    }

}