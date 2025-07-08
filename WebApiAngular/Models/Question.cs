namespace WebApiAngular.Models
{
    // Models/Question.cs
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }  // "MultipleChoice" or "TrueFalse"
        public int Points { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public List<Option> Options { get; set; } = new();
    }
}
