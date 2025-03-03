namespace Quizzik_Project.DTO
{
    public class QuizAttemptSubmissionDTO
    {
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public Dictionary<int, int> Answers { get; set; } // Key: QuestionID, Value: Selected OptionID
    }
}
