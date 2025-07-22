using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archi_applicatives_MSAFE.msafe.com.models;

[Table("reservation")] // 👈 correction ici
public class Reservation
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Client")]
    public int ClientId { get; set; }
    public Client Client { get; set; }

    public List<ChambreReservation> ChambreReservations { get; set; } = new();

    [Required]
    public DateTime DateDebut { get; set; }

    [Required]
    public DateTime DateFin { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal MontantTotal { get; set; }

    public bool EstAnnulee { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}