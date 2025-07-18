﻿// Controllers/StudentExamsController.cs
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

    [HttpGet("results/{resultId}")]
    public async Task<IActionResult> GetExamByResultId(int resultId)
    {
        var result = await _context.Results
            .Include(r => r.Exam)
                .ThenInclude(e => e.Questions)
                    .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(r => r.Id == resultId);

        if (result == null)
            return NotFound();

        // Ensure the requesting user owns the result
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (result.UserId != userId)
            return Forbid();

        var exam = result.Exam;

        return Ok(new ExamResponseDto
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
                    IsCorrect = o.IsCorrect // Optional: you might want to hide this for students
                }).ToList()
            }).ToList()
        });
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

    [HttpGet("results")]
    public async Task<IActionResult> GetAllResults()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var results = await _context.Results
            .Where(r => r.UserId == userId)
            .Include(r => r.Exam)
            .Select(r => new
            {
                r.Id,
                r.ExamId,
                ExamTitle = r.Exam.Title,
                r.Score,
                r.TotalPoints,
                r.StartTime,
                r.EndTime
            })
            .ToListAsync();

        return Ok(results);
    }


}