namespace WebApiAngular.Models
{
    // Models/Exam.cs
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }

        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public List<Question> Questions { get; set; } = new();
    }
}
