using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Quizzik_Project.Models
{
    public class Quiz
    {
        [Key]
        public int QuizID { get; set; }

        [Required,StringLength(100)]
        public string? Title { get; set; }

        [StringLength(255)]
        public string Description { get; set; }
        [Required]
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        [ForeignKey("UserID")]
        public User User { get; set; }

        public ICollection<Question> Questions { get; set; }

        public ICollection<QuizAttempt> QuizAttempts { get; set; }

        public Leaderboard Leaderboard { get; set; }
        //public ICollection<Leaderboard> Leaderboards { get; set; }
    }
}
