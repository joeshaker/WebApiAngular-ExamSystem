// Controllers/ExamsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiAngular.DbContexts;
using WebApiAngular.DtoModel;
using WebApiAngular.Models;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ExamsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExamsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateExam(ExamDto examDto)
    {
        var exam = new Exam
        {
            Title = examDto.Title,
            Description = examDto.Description,
            DurationMinutes = examDto.DurationMinutes,
            CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        };

        _context.Exams.Add(exam);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetExam), new { id = exam.Id }, exam);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetExam(int id)
    {
        var exam = await _context.Exams
            .Include(e => e.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exam == null)
            return NotFound();

        var response = new ExamResponseDto
        {
            Id = exam.Id,
            Title = exam.Title,
            Description = exam.Description,
            DurationMinutes = exam.DurationMinutes,
            Questions = exam.Questions.Select(q => new QuestionResponseDto
            {
                Id = q.Id,
                Text = q.Text,
                Type = q.Type,
                Points = q.Points,
                Options = q.Options.Select(o => new OptionResponseDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            }).ToList()
        };

        return Ok(response);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var exams = await _context.Exams
            .Include(e => e.Questions)
                .ThenInclude(q => q.Options)
            .Select(e => new ExamResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                DurationMinutes = e.DurationMinutes,
                Questions = e.Questions.Select(q => new QuestionResponseDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Type = q.Type,
                    Points = q.Points,
                    Options = q.Options.Select(o => new OptionResponseDto
                    {
                        Id = o.Id,
                        Text = o.Text,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                }).ToList()
            })
            .ToListAsync();

        return Ok(exams);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExam(int id, ExamDto examDto)
    {
        var exam = await _context.Exams.FindAsync(id);
        if (exam == null) return NotFound();

        exam.Title = examDto.Title;
        exam.Description = examDto.Description;
        exam.DurationMinutes = examDto.DurationMinutes;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExam(int id)
    {
        var exam = await _context.Exams.FindAsync(id);
        if (exam == null) return NotFound();

        _context.Exams.Remove(exam);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}