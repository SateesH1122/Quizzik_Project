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
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class OptionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly EFCoreDbContext _context;

        public OptionsController(IMapper mapper, EFCoreDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Options
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OptionDTO>>> GetAll()
        {
            try
            {
                var options = await _context.Options.ToListAsync();
                var optionDTOs = _mapper.Map<List<OptionDTO>>(options);
                return Ok(optionDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OptionDTO>> GetById(int id)
        {
            try
            {
                var option = await _context.Options.FindAsync(id);
                if (option == null)
                {
                    return NotFound();
                }
                var optionDTO = _mapper.Map<OptionDTO>(option);
                return Ok(optionDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<OptionDTO>> Create(OptionDTO optionDTO)
        {
            try
            {
                var option = _mapper.Map<Option>(optionDTO);
                _context.Options.Add(option);
                await _context.SaveChangesAsync();
                optionDTO.OptionID = option.OptionID;
                return CreatedAtAction(nameof(GetById), new { id = option.OptionID }, optionDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OptionDTO optionDTO)
        {
            try
            {
                if (id != optionDTO.OptionID)
                {
                    return BadRequest();
                }

                var option = _mapper.Map<Option>(optionDTO);
                _context.Entry(option).State = EntityState.Modified;
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
                var option = await _context.Options.FindAsync(id);
                if (option == null)
                {
                    return NotFound();
                }

                _context.Options.Remove(option);
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
