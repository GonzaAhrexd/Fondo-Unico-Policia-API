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
    public class BancosController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public BancosController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Bancos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bancos>>> GetBancos()
        {
            return await _context.Bancos.ToListAsync();
        }

        // GET: api/Bancos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bancos>> GetBancos(int id)
        {
            var bancos = await _context.Bancos.FindAsync(id);

            if (bancos == null)
            {
                return NotFound();
            }

            return bancos;
        }

        // PUT: api/Bancos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBancos(int id, Bancos bancos)
        {
            if (id != bancos.Id)
            {
                return BadRequest();
            }

            _context.Entry(bancos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BancosExists(id))
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

        // POST: api/Bancos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bancos>> PostBancos(Bancos bancos)
        {
            _context.Bancos.Add(bancos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBancos", new { id = bancos.Id }, bancos);
        }

        // DELETE: api/Bancos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBancos(int id)
        {
            var bancos = await _context.Bancos.FindAsync(id);
            if (bancos == null)
            {
                return NotFound();
            }

            _context.Bancos.Remove(bancos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BancosExists(int id)
        {
            return _context.Bancos.Any(e => e.Id == id);
        }
    }
}
