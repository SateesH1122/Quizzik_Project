using AutoMapper;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizzik_Project.DTO;
using Quizzik_Project.Models;
using Microsoft.AspNetCore.Authorization;

namespace Quizzik_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly EFCoreDbContext _context;
        private readonly ILogger<LeaderboardsController> _logger;

        public LeaderboardsController(IMapper mapper, EFCoreDbContext context, ILogger<LeaderboardsController> logger)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        // Method to get leaderboard by quiz ID
        [HttpGet("Quiz/{quizId}")]
        public async Task<ActionResult<IEnumerable<QuizAttemptDTO>>> GetLeaderboardByQuizId(int quizId)
        {
            try
            {
                _logger.LogInformation("Getting leaderboard for quiz ID {QuizId}", quizId);

                var quizAttempts = await _context.QuizAttempts
                    .Where(qa => qa.QuizID == quizId)
                    .OrderByDescending(qa => qa.Score)
                    .ThenBy(qa => qa.AttemptDate)
                    .ToListAsync();

                var quizAttemptDTOs = _mapper.Map<List<QuizAttemptDTO>>(quizAttempts);

                _logger.LogInformation("Successfully retrieved leaderboard for quiz ID {QuizId}", quizId);

                return Ok(quizAttemptDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting leaderboard for quiz ID {QuizId}", quizId);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
