namespace WebApiAngular.DtoModel
{
    // DTOs/QuestionDto.cs
    public class QuestionDto
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public int Points { get; set; }
        public List<OptionDto> Options { get; set; } = new();
    }

    // Used for input only
    public class OptionDto
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }

    // Output DTO: includes Ids
    public class QuestionResponseDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public int Points { get; set; }
        public List<OptionResponseDto> Options { get; set; } = new();
    }

    public class OptionResponseDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}
