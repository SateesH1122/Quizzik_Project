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
    public class QuestionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly EFCoreDbContext _context;

        public QuestionsController(IMapper mapper, EFCoreDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetAll()
        {
            try
            {
                var questions = await _context.Questions.ToListAsync();
                var questionDTOs = _mapper.Map<List<QuestionDTO>>(questions);
                return Ok(questionDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDTO>> GetById(int id)
        {
            try
            {
                var question = await _context.Questions.FindAsync(id);
                if (question == null)
                {
                    return NotFound();
                }
                var questionDTO = _mapper.Map<QuestionDTO>(question);
                return Ok(questionDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<QuestionDTO>> Create(QuestionDTO questionDTO)
        {
            try
            {
                var question = _mapper.Map<Question>(questionDTO);
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                questionDTO.QuestionID = question.QuestionID;
                return CreatedAtAction(nameof(GetById), new { id = question.QuestionID }, questionDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, QuestionDTO questionDTO)
        {
            try
            {
                if (id != questionDTO.QuestionID)
                {
                    return BadRequest();
                }

                var question = _mapper.Map<Question>(questionDTO);
                _context.Entry(question).State = EntityState.Modified;
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
                var question = await _context.Questions.FindAsync(id);
                if (question == null)
                {
                    return NotFound();
                }

                _context.Questions.Remove(question);
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
