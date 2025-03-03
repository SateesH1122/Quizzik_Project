using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Quizzik_Project.Models
{
    public class QuizAttempt
    {
        [Key]
        public int AttemptID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int QuizID { get; set; }

        [Required]
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; } = DateTime.Now;

        //Navigation Properties

        [ForeignKey("QuizID")]
        public Quiz Quiz { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

    }
}
