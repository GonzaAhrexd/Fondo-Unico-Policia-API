using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FondoUnicoSistemaCompleto.Context;
using Microsoft.AspNetCore.Authorization;
using SistemaFondoUnicoAPI.Models;

namespace FondoUnicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RenglonesEntregasController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public RenglonesEntregasController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/RenglonesEntregas
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RenglonesEntrega>>> GetRenglonesEntregas()
        {
            return await _context.RenglonesEntregas.ToListAsync();
        }

        // GET: api/RenglonesEntregas/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RenglonesEntrega>> GetRenglonesEntrega(int id)
        {
            var renglonesEntrega = await _context.RenglonesEntregas.FindAsync(id);

            if (renglonesEntrega == null)
            {
                return NotFound();
            }

            return renglonesEntrega;
        }

        // PUT: api/RenglonesEntregas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutRenglonesEntrega(int id, RenglonesEntrega renglonesEntrega)
        {
            if (id != renglonesEntrega.Id)
            {
                return BadRequest();
            }

            _context.Entry(renglonesEntrega).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RenglonesEntregaExists(id))
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

        // POST: api/RenglonesEntregas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RenglonesEntrega>> PostRenglonesEntrega(RenglonesEntrega renglonesEntrega)
        {
            _context.RenglonesEntregas.Add(renglonesEntrega);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRenglonesEntrega", new { id = renglonesEntrega.Id }, renglonesEntrega);
        }

        // DELETE: api/RenglonesEntregas/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRenglonesEntrega(int id)
        {
            var renglonesEntrega = await _context.RenglonesEntregas.FindAsync(id);
            if (renglonesEntrega == null)
            {
                return NotFound();
            }

            _context.RenglonesEntregas.Remove(renglonesEntrega);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RenglonesEntregaExists(int id)
        {
            return _context.RenglonesEntregas.Any(e => e.Id == id);
        }
    }
}
