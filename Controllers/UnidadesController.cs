using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FondoUnicoSistemaCompleto.Context;
using FondoUnicoSistemaCompleto.Models;

namespace FondoUnicoSistemaCompleto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnidadesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UnidadesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Unidades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Unidades>>> GetUnidades()
        {
            return await _context.Unidades.ToListAsync();
        }

        // GET: api/Unidades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Unidades>> GetUnidades(int id)
        {
            var unidades = await _context.Unidades.FindAsync(id);

            if (unidades == null)
            {
                return NotFound();
            }

            return unidades;
        }

        // PUT: api/Unidades/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnidades(int id, Unidades unidades)
        {
            if (id != unidades.Id)
            {
                return BadRequest();
            }

            _context.Entry(unidades).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UnidadesExists(id))
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

        // POST: api/Unidades
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Unidades>> PostUnidades(Unidades unidades)
        {
            _context.Unidades.Add(unidades);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUnidades", new { id = unidades.Id }, unidades);
        }

        // DELETE: api/Unidades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnidades(int id)
        {
            var unidades = await _context.Unidades.FindAsync(id);
            if (unidades == null)
            {
                return NotFound();
            }

            _context.Unidades.Remove(unidades);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UnidadesExists(int id)
        {
            return _context.Unidades.Any(e => e.Id == id);
        }
    }
}
