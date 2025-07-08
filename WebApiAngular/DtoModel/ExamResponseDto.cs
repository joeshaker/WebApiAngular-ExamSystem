namespace WebApiAngular.DtoModel
{
    public class ExamResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public List<QuestionResponseDto> Questions { get; set; } = new();
    }
}
