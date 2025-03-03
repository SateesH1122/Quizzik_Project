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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QuizAttemptsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly EFCoreDbContext _context;

        public QuizAttemptsController(IMapper mapper, EFCoreDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/QuizAttempts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizAttemptDTO>>> GetAll()
        {
            try
            {
                var quizAttempts = await _context.QuizAttempts.ToListAsync();
                var quizAttemptDTOs = _mapper.Map<List<QuizAttemptDTO>>(quizAttempts);
                return Ok(quizAttemptDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<QuizAttemptDTO>> GetById(int id)
        {
            try
            {
                var quizAttempt = await _context.QuizAttempts.FindAsync(id);
                if (quizAttempt == null)
                {
                    return NotFound();
                }
                var quizAttemptDTO = _mapper.Map<QuizAttemptDTO>(quizAttempt);
                return Ok(quizAttemptDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var quizAttempt = await _context.QuizAttempts.FindAsync(id);
                if (quizAttempt == null)
                {
                    return NotFound();
                }

                _context.QuizAttempts.Remove(quizAttempt);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("WithOptions")]
        public async Task<ActionResult<Dictionary<int, QuestionWithOptionsDTO>>> GetQuestionsWithOptions()
        {
            try
            {
                var questions = await _context.Questions
                    .Include(q => q.Options)
                    .Include(q => q.Quiz)
                    .ToListAsync();

                var result = new Dictionary<int, QuestionWithOptionsDTO>();

                foreach (var question in questions)
                {
                    var questionWithOptionsDTO = new QuestionWithOptionsDTO
                    {
                        QuestionID = question.QuestionID,
                        QuestionText = question.QuestionText,
                        Options = question.Options.ToDictionary(o => o.OptionID, o => o.OptionText),
                        QuizID = question.QuizID,
                        UserID = question.Quiz.UserID
                    };

                    result.Add(question.QuestionID, questionWithOptionsDTO);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPost("Submit")]
        public async Task<ActionResult<QuizAttemptDTO>> SubmitQuizAttempt(QuizAttemptSubmissionDTO submissionDTO)
        {
            try
            {
                var existingAttempt = await _context.QuizAttempts
                    .FirstOrDefaultAsync(qa => qa.UserID == submissionDTO.UserID && qa.QuizID == submissionDTO.QuizID);

                if (existingAttempt != null)
                {
                    return BadRequest("User has already attempted this quiz.");
                }

                var quiz = await _context.Quizzes
                    .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                    .FirstOrDefaultAsync(q => q.QuizID == submissionDTO.QuizID);

                if (quiz == null)
                {
                    return NotFound("Quiz not found.");
                }

                int score = 0;

                foreach (var question in quiz.Questions)
                {
                    if (submissionDTO.Answers.TryGetValue(question.QuestionID, out int selectedOptionID))
                    {
                        var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                        if (correctOption != null && correctOption.OptionID == selectedOptionID)
                        {
                            score++;
                        }
                    }
                }

                var quizAttempt = new QuizAttempt
                {
                    UserID = submissionDTO.UserID,
                    QuizID = submissionDTO.QuizID,
                    Score = score,
                    AttemptDate = DateTime.Now
                };

                _context.QuizAttempts.Add(quizAttempt);
                await _context.SaveChangesAsync();

                var quizAttemptDTO = _mapper.Map<QuizAttemptDTO>(quizAttempt);

                return Ok(quizAttemptDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
