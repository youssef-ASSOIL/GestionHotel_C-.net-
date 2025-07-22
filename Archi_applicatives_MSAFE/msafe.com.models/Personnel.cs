using System.ComponentModel.DataAnnotations;

namespace Archi_applicatives_MSAFE.msafe.com.models;

public class Personnel
{
    [Key]
    private int id;
    TypeDePersonnel typeDePersonnel;
    string nom;
    string prenom;
    public Personnel()
    {
        
    }

    public Personnel(int id, TypeDePersonnel typeDePersonnel, string nom, string prenom)
    {
        this.id = id;
        this.typeDePersonnel = typeDePersonnel;
        this.nom = nom;
        this.prenom = prenom;
    }

    public int Id => id;

    public TypeDePersonnel TypeDePersonnel => typeDePersonnel;

    public string Nom => nom;

    public string Prenom => prenom;
}