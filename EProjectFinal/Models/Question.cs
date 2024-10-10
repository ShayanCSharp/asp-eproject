using System.ComponentModel.DataAnnotations;

namespace EProjectFinal.Models
{
    public partial class Question
    {
        public int QuestionId { get; set; }

        [Required]
        public int TestId { get; set; }

        [Required]
        [StringLength(500)] // Assuming a max length for question text
        public string QuestionText { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Option1 { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Option2 { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Option3 { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Option4 { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string CorrectAnswer { get; set; } = null!;

        [Required]
        [Range(1, 5)] // Assuming marks range from 1 to 5; adjust as needed
        public int Marks { get; set; }

        public virtual ICollection<CandidateAnswer> CandidateAnswers { get; set; } = new List<CandidateAnswer>();

        public virtual Test? Test { get; set; } = null!;
    }
}
