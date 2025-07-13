// Controllers/ResultsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiAngular.DbContexts;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ResultsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("my-results")]
    public async Task<IActionResult> GetMyResults()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var results = await _context.Results
            .Where(r => r.UserId == userId)
            .Include(r => r.Exam)
            .ToListAsync();

        return Ok(results);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("exam/{examId}")]
    public async Task<IActionResult> GetExamResults(int examId)
    {
        var results = await _context.Results
            .Where(r => r.ExamId == examId)
            .Include(r => r.User)
            .Include(r => r.Exam)
            .ToListAsync();

        return Ok(results);
    }
    // result persentage calculation
    [HttpGet("exam/{examId}/percentage")]
    public async Task<IActionResult> GetExamResultPercentage(int examId)
    {
        var results = await _context.Results
            .Where(r => r.ExamId == examId)
            .ToListAsync();
        if (results.Count == 0)
            return NotFound("No results found for this exam.");
        var totalPoints = results.Sum(r => r.TotalPoints);
        var maxPoints = results.Sum(r => 90);
        if (maxPoints == 0)
            return Ok(new { Percentage = 0 });
        var percentage = (double)totalPoints / maxPoints * 100;
        return Ok(new { Percentage = percentage });
    }
}