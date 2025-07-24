using System;
using System.Collections.Generic;
using Archi_applicatives_MSAFE.msafe.com.models;

namespace Archi_applicatives_MSAFE.msafe.com.DTOs
{
    public class ReservationRequest
    {
        public Client Client { get; set; } = new();
        public List<int> ChambreIds { get; set; } = new();
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string CarteBleue { get; set; } = string.Empty;
    }
}