using System;
using System.ComponentModel.DataAnnotations;

namespace Airport_REST_API.Shared.DTO
{
    public class DeparturesDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 6)]
        public string Number { get; set; }

        [Required]
        public string DepartureTime { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int? CrewId { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int? AircraftId { get; set; }
    }
}
