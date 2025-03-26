using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FondoUnicoSistemaCompleto.Context;
using SistemaFondoUnicoAPI.Models;

namespace FondoUnicoSistemaCompleto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormulariosController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public FormulariosController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Formularios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Formularios>>> GetFormularios()
        {
            return await _context.Formularios.ToListAsync();
        }

        // GET: api/Formularios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Formularios>> GetFormularios(int id)
        {
            var formularios = await _context.Formularios.FindAsync(id);

            if (formularios == null)
            {
                return NotFound();
            }

            return formularios;
        }

        // PUT: api/Formularios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFormularios(int id, Formularios formularios)
        {
            if (id != formularios.Id)
            {
                return BadRequest();
            }

            _context.Entry(formularios).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormulariosExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Formularios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Formularios>> PostFormularios(Formularios formularios)
        {
            _context.Formularios.Add(formularios);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFormularios", new { id = formularios.Id }, formularios);
        }

        // DELETE: api/Formularios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormularios(int id)
        {
            var formularios = await _context.Formularios.FindAsync(id);
            if (formularios == null)
            {
                return NotFound();
            }

            _context.Formularios.Remove(formularios);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FormulariosExists(int id)
        {
            return _context.Formularios.Any(e => e.Id == id);
        }
    }
}
