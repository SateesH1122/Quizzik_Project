using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Quizzik_Project.Models
{
    public class Option
    {
        [Key]
        public int OptionID { get; set; }

        [Required]
        public int QuestionID { get; set; }

        [Required,StringLength(255)]
        public string? OptionText { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        [ForeignKey("QuestionID")]
        public Question Question { get; set; }
    }
}
