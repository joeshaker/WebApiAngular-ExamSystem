namespace WebApiAngular.Models
{
    // Models/Result.cs
    public class Result
    {
        public int Id { get; set; }
        public float Score { get; set; }
        public float TotalPoints { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public List<Answer> Answers { get; set; } = new();
    }
}
