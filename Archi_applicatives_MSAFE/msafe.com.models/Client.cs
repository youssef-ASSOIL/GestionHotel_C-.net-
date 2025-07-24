using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archi_applicatives_MSAFE.msafe.com.models;

[Table("client")] // ok si ta table dans la BDD s'appelle bien "client"
public class Client
{
    [Key]
    public int Id { get; set; }

    // 👇 Ces propriétés utilisent des champs privés, bien pour validation
    private string _nom = string.Empty;
    public string Nom
    {
        get => _nom;
        set => _nom = value ?? throw new ArgumentNullException(nameof(value));
    }

    private string _prenom = string.Empty;
    public string Prenom
    {
        get => _prenom;
        set => _prenom = value ?? throw new ArgumentNullException(nameof(value));
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => _email = value ?? throw new ArgumentNullException(nameof(value));
    }

    [Required]
    public string Telephone { get; set; }

    // 👇 Navigation vers les réservations
    public List<Reservation> Reservations { get; set; } = new List<Reservation>();

    public Client() { }

    public Client(int id, string nom, string prenom, string email, string telephone)
    {
        Id = id;
        Nom = nom;
        Prenom = prenom;
        Email = email;
        Telephone = telephone;
    }
}
