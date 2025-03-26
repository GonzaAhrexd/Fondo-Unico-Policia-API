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
    public class LocalidadesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public LocalidadesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Localidades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Localidades>>> GetLocalidades()
        {
            return await _context.Localidades.ToListAsync();
        }

        // GET: api/Localidades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Localidades>> GetLocalidades(int id)
        {
            var localidades = await _context.Localidades.FindAsync(id);

            if (localidades == null)
            {
                return NotFound();
            }

            return localidades;
        }

        // PUT: api/Localidades/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocalidades(int id, Localidades localidades)
        {
            if (id != localidades.Id)
            {
                return BadRequest();
            }

            _context.Entry(localidades).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocalidadesExists(id))
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

        // POST: api/Localidades
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Localidades>> PostLocalidades(Localidades localidades)
        {
            _context.Localidades.Add(localidades);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocalidades", new { id = localidades.Id }, localidades);
        }

        // DELETE: api/Localidades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocalidades(int id)
        {
            var localidades = await _context.Localidades.FindAsync(id);
            if (localidades == null)
            {
                return NotFound();
            }

            _context.Localidades.Remove(localidades);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocalidadesExists(int id)
        {
            return _context.Localidades.Any(e => e.Id == id);
        }
    }
}
