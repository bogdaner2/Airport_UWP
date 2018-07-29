using System.ComponentModel.DataAnnotations;

namespace Aiport_UWP.DTO
{
    public class StewardessDTO
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
    }
}
