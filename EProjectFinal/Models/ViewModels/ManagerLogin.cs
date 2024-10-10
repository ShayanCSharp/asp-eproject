using System.ComponentModel.DataAnnotations;

namespace EProjectFinal.Models.ViewModels
{
    public class ManagerLogin
    {
        [Required]
        [StringLength(20,MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20,MinimumLength = 8)]
        public string Password { get; set; } = null!;

    }
}
