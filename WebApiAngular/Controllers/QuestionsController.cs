using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAngular.DbContexts;
using WebApiAngular.DtoModel;
using WebApiAngular.Models;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/exams/{examId}/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public QuestionsController(AppDbContext context)
    {
        _context = context;
    }

    // POST: Add Question
    [HttpPost]
    public async Task<IActionResult> AddQuestion(int examId, QuestionDto questionDto)
    {
        var question = new Question
        {
            Text = questionDto.Text,
            Type = questionDto.Type,
            Points = questionDto.Points,
            ExamId = examId
        };

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        // Add related options
        foreach (var optionDto in questionDto.Options)
        {
            var option = new Option
            {
                Text = optionDto.Text,
                IsCorrect = optionDto.IsCorrect,
                QuestionId = question.Id
            };
            _context.Options.Add(option);
        }

        await _context.SaveChangesAsync();

        // Prepare response DTO
        var responseDto = new QuestionResponseDto
        {
            Id = question.Id,
            Text = question.Text,
            Type = question.Type,
            Points = question.Points,
            Options = await _context.Options
                .Where(o => o.QuestionId == question.Id)
                .Select(o => new OptionResponseDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToListAsync()
        };

        return CreatedAtAction(nameof(GetQuestion), new { examId = examId, id = question.Id }, responseDto);
    }

    // GET: Get Question by Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuestion(int examId, int id)
    {
        var question = await _context.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id && q.ExamId == examId);

        if (question == null)
            return NotFound();

        var responseDto = new QuestionResponseDto
        {
            Id = question.Id,
            Text = question.Text,
            Type = question.Type,
            Points = question.Points,
            Options = question.Options.Select(o => new OptionResponseDto
            {
                Id = o.Id,
                Text = o.Text,
                IsCorrect = o.IsCorrect
            }).ToList()
        };

        return Ok(responseDto);
    }
}
