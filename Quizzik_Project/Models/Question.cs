using Microsoft.CodeAnalysis.Options;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Quizzik_Project.Models
{
    public class Question
    {
        [Key]
        public int QuestionID { get; set; }

        [Required]
        public int QuizID { get; set; }

        [Required,StringLength(255)]
        public string? QuestionText { get; set; }

        [Required,StringLength(20)]
        public string? QuestionType { get; set; }

        [Required,StringLength(10)]
        public string? DifficultyLevel { get; set; }

        [ForeignKey("QuizID")]
        public Quiz Quiz { get; set; }

        public ICollection<Option> Options { get; set; }
    }
}
