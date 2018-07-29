using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport_REST_API.Shared.DTO
{
    public class FlightDTO
    {
        public int Id { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        [MinLength(3)]
        public string PointOfDeparture { get; set; }

        [Required]
        public string DepartureTime { get; set; }

        [MinLength(3)]
        public string Destination { get; set; }

        [Required]
        public string ArrivelTime { get; set; }

        [Required]
        public List<int> TicketsId { get; set; }
    }
}
