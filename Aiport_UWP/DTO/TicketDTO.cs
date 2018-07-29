using System.ComponentModel.DataAnnotations;

namespace Aiport_UWP.DTO
{
    public class TicketDTO
    {
        public int Id { get; set; }
        [Required]
        [Range(1.0, 10000.0, ErrorMessage = "Price must be between $1 and $10000")]
        public double Price { get; set; }

        [Required]
        [MinLength(4,ErrorMessage = "Number must be more than 4 letters")]
        public string Number { get; set; }

        public override string ToString()
        {
            return $"Id : {Id} Number : {Number}";
        }
    }
}
