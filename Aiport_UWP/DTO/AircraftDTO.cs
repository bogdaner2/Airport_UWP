﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Aiport_UWP.DTO
{
    public class AircraftDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int TypeId { get; set; }

        public string ReleseDate { get; set; }
        public string Lifetime { get; set; }

        public override string ToString()
        {
            return $"Id : {Id} Name : {Name}";
        }
    }
}
