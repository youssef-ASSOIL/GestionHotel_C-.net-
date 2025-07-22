using System.ComponentModel.DataAnnotations.Schema;

namespace Archi_applicatives_MSAFE.msafe.com.models;

[Table("chambrereservation")] // 🔧 mapping explicite à la vraie table MySQL
public class ChambreReservation
{
    [ForeignKey("Chambre")]
    public int ChambreId { get; set; }
    public Chambre Chambre { get; set; }

    [ForeignKey("Reservation")]
    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; }
}