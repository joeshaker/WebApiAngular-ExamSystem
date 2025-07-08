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
}