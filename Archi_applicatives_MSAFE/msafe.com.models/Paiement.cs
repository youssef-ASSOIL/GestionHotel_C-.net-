namespace Archi_applicatives_MSAFE.msafe.com.models;

public class Paiement
{ 
    int id_paiement;
    double montant_paiement;
    DateTime date_paiement;
    ModePaiement mode_paiement;
    private bool est_effectuer;
    
    public Paiement(){}

    public Paiement(int idPaiement, double montantPaiement, DateTime datePaiement, ModePaiement modePaiement, bool estEffectuer)
    {
        id_paiement = idPaiement;
        montant_paiement = montantPaiement;
        date_paiement = datePaiement;
        mode_paiement = modePaiement;
        est_effectuer = estEffectuer;
    }

    public int IdPaiement
    {
        get => id_paiement;
        set => id_paiement = value;
    }

    public double MontantPaiement
    {
        get => montant_paiement;
        set => montant_paiement = value;
    }

    public DateTime DatePaiement
    {
        get => date_paiement;
        set => date_paiement = value;
    }

    public ModePaiement ModePaiement
    {
        get => mode_paiement;
        set => mode_paiement = value;
    }

    public bool EstEffectuer
    {
        get => est_effectuer;
        set => est_effectuer = value;
    }
}