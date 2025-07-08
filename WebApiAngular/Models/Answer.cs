namespace WebApiAngular.Models
{
    // Models/Answer.cs
    public class Answer
    {
        public int Id { get; set; }

        public int ResultId { get; set; }
        public Result Result { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int SelectedOptionId { get; set; }
        public Option SelectedOption { get; set; }
    }
}
