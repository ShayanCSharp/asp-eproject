using System.ComponentModel.DataAnnotations;

namespace EProjectFinal.Models.ViewModels
{
    public class CandidateLogin
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [StringLength(25,MinimumLength = 8)]
        public string Password { get; set; } = null!;

    }

}