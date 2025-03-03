namespace Quizzik_Project.DTO
{
    public class QuestionDTO
    {
        public int QuestionID { get; set; } 
        public int QuizID { get; set; }
        public string? QuestionText { get; set; }
        public string? QuestionType { get; set; }
        public string? DifficultyLevel { get; set; }
    }
}
