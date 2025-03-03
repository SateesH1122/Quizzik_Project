namespace Quizzik_Project.DTO
{
    public class QuizAttemptDTO
    {
        public int AttemptID { get; set; }
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public int Score { get; set; }

        public DateTime AttemptDate { get; set; } = DateTime.Now;
    }
}
