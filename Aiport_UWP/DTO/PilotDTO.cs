using System.ComponentModel.DataAnnotations;

namespace Aiport_UWP.DTO
{
    public class PilotDTO
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }

        [Required]
        [Range(1,50)]
        public int Experience { get; set; }

        public override string ToString()
        {
            return $"Id : {Id} Fullname : {FirstName} {LastName}";
        }
    }
}
