using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Quizzik_Project.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required, StringLength(50)]
        public string? Username { get; set; }
        [Required, StringLength(255)]
        public string? Password { get; set; }
        [Required, EmailAddress, StringLength(100)]
        public string? Email { get; set; }
        [Required, StringLength(10)]
        [RegularExpression("^(Admin|Student)$", ErrorMessage = "Role must be either 'Admin' or 'Student'.")]
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Quiz> Quizzes { get; set; }
        public ICollection<QuizAttempt> QuizAttempts { get; set; }
        public ICollection<Leaderboard> Leaderboards { get; set; }
    }
}
