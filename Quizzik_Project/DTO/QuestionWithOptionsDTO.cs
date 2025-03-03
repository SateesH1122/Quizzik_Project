namespace Quizzik_Project.DTO
{
    public class QuestionWithOptionsDTO
    {
        public int QuestionID { get; set; }
        public string? QuestionText { get; set; }
        public Dictionary<int, string>? Options { get; set; }
        public int QuizID { get; set; }
        public int UserID { get; set; }
    }
}
