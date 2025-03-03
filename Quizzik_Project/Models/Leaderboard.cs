using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Quizzik_Project.Models
{
    public class Leaderboard
    {
        [Key]
        public int LeaderboardID { get; set; }

        [Required]
        public int QuizID { get; set; }

        [Required]

        [ForeignKey("QuizID")]
        public Quiz Quiz { get; set; }

        public ICollection<QuizAttempt> QuizAttempts { get; set; }

    }
}
