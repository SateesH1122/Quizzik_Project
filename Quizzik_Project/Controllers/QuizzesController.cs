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
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly EFCoreDbContext _context;

        public QuizzesController(IMapper mapper, EFCoreDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Quizzes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizDTO>>> GetAll()
        {
            try
            {
                var quizzes = await _context.Quizzes.ToListAsync();
                var quizDTOs = _mapper.Map<List<QuizDTO>>(quizzes);
                return Ok(quizDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDTO>> GetById(int id)
        {
            try
            {
                var quiz = await _context.Quizzes.FindAsync(id);
                if (quiz == null)
                {
                    return NotFound();
                }
                var quizDTO = _mapper.Map<QuizDTO>(quiz);
                return Ok(quizDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<QuizDTO>> Create(QuizDTO quizDTO)
        {
            try
            {
                var quiz = _mapper.Map<Quiz>(quizDTO);
                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();
                quizDTO.QuizID = quiz.QuizID;
                return CreatedAtAction(nameof(GetById), new { id = quiz.QuizID }, quizDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, QuizDTO quizDTO)
        {
            try
            {
                if (id != quizDTO.QuizID)
                {
                    return BadRequest();
                }

                var quiz = _mapper.Map<Quiz>(quizDTO);
                _context.Entry(quiz).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var quiz = await _context.Quizzes.FindAsync(id);
                if (quiz == null)
                {
                    return NotFound();
                }

                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
