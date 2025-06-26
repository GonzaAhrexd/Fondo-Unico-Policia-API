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
    public class ArqueosController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ArqueosController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Arqueos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Arqueos>>> GetArqueos()
        {
            return await _context.Arqueos.ToListAsync();
        }

        // GET: api/Arqueos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Arqueos>> GetArqueos(int id)
        {
            var arqueos = await _context.Arqueos.FindAsync(id);

            if (arqueos == null)
            {
                return NotFound();
            }

            return arqueos;
        }

        // PUT: api/Arqueos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArqueos(int id, Arqueos arqueos)
        {
            if (id != arqueos.Id)
            {
                return BadRequest();
            }

            _context.Entry(arqueos).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArqueosExists(id))
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

        // POST: api/Arqueos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Arqueos>> PostArqueos(Arqueos arqueos)
        {
            _context.Arqueos.Add(arqueos);

            var registroEntrega = new RegistroEntregas
            {
                Unidad = arqueos.Unidad,
                Fecha = arqueos.Hasta,
                TipoEntrega = arqueos.TipoDeFormulario,
                cantidadActual = arqueos.CantidadRestante
            };
            _context.RegistroEntregas.Add(registroEntrega);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArqueos", new { id = arqueos.Id }, arqueos);
        }

        // DELETE: api/Arqueos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArqueos(int id)
        {
            var arqueos = await _context.Arqueos.FindAsync(id);
            if (arqueos == null)
            {
                return NotFound();
            }

            _context.Arqueos.Remove(arqueos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArqueosExists(int id)
        {
            return _context.Arqueos.Any(e => e.Id == id);
        }
    }
}
