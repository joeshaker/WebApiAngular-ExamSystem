// Controllers/StudentExamsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiAngular.DbContexts;
using WebApiAngular.DtoModel;
using WebApiAngular.Models;

[Authorize(Roles = "Student")]
[ApiController]
[Route("api/student/[controller]")]
public class StudentExamsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentExamsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableExams()
    {
        var exams = await _context.Exams.ToListAsync();
        return Ok(exams);
    }

    [HttpPost("{examId}/start")]
    public async Task<IActionResult> StartExam(int examId)
    {
        var exam = await _context.Exams.FindAsync(examId);
        if (exam == null) return NotFound();

        var result = new Result
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            ExamId = examId,
            StartTime = DateTime.UtcNow,
            TotalPoints = await _context.Questions
                .Where(q => q.ExamId == examId)
                .SumAsync(q => q.Points)
        };

        _context.Results.Add(result);
        await _context.SaveChangesAsync();
        return Ok(new { resultId = result.Id });
    }

    [HttpPost("results/{resultId}/submit")]
    public async Task<IActionResult> SubmitExam(int resultId, List<AnswerSubmissionDto> answers)
    {
        var result = await _context.Results
            .Include(r => r.Exam)
            .FirstOrDefaultAsync(r => r.Id == resultId);

        if (result == null) return NotFound();
        if (result.UserId != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            return Forbid();

        float score = 0;
        foreach (var answer in answers)
        {
            var correctOptionId = await _context.Options
                .Where(o => o.QuestionId == answer.QuestionId && o.IsCorrect)
                .Select(o => o.Id)
                .FirstOrDefaultAsync();

            if (answer.SelectedOptionId == correctOptionId)
            {
                score += await _context.Questions
                    .Where(q => q.Id == answer.QuestionId)
                    .Select(q => q.Points)
                    .FirstOrDefaultAsync();
            }

            _context.Answers.Add(new Answer
            {
                ResultId = resultId,
                QuestionId = answer.QuestionId,
                SelectedOptionId = answer.SelectedOptionId
            });
        }

        result.Score = score;
        result.EndTime = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { score = result.Score, total = result.TotalPoints });
    }
}