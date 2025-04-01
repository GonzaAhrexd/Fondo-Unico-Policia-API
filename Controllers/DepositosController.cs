using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FondoUnicoSistemaCompleto.Context;
using FondoUnicoSistemaCompleto.Models;
using Microsoft.AspNetCore.Authorization;

namespace FondoUnicoSistemaCompleto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositosController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public DepositosController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Depositos
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Depositos>>> GetDepositos()
        {
            return await _context.Depositos.ToListAsync();
        }

        // GET: api/Depositos/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Depositos>> GetDepositos(int id)
        {
            var depositos = await _context.Depositos.FindAsync(id);

            if (depositos == null)
            {
                return NotFound();
            }

            return depositos;
        }

        // PUT: api/Depositos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutDepositos(int id, Depositos depositos)
        {
            if (id != depositos.Id)
            {
                return BadRequest();
            }

            _context.Entry(depositos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepositosExists(id))
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

        // POST: api/Depositos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Depositos>> PostDepositos(Depositos depositos)
        {
            _context.Depositos.Add(depositos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepositos", new { id = depositos.Id }, depositos);
        }

        [HttpGet("{unidad}/{fechaInicio}/{fechaFinal}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Depositos>>> GetDepositosPorUnidadFecha(string unidad, DateTime fechaInicio, DateTime fechaFinal)
        {
            if(unidad == "Listar todo")
            {
                // Retorna un arreglo con los depositos que cumplan con la fecha de inicio y final
                return await _context.Depositos.Where(e => e.Fecha >= fechaInicio && e.Fecha <= fechaFinal).ToListAsync();
            }
            // Retorna un arreglo con los depositos que cumplan con la unidad y la fecha de inicio y final
            return await _context.Depositos.Where(e => e.Unidad == unidad && e.Fecha >= fechaInicio && e.Fecha <= fechaFinal).ToListAsync();


        }
        [HttpGet("buscar-por-nro-deposito/{nroDeposito}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Depositos>>> GetDepositosPorNroTicket(int nroDeposito)
        {
            return await _context.Depositos.Where(e => e.NroDeposito == nroDeposito).ToListAsync();
        }

        // DELETE: api/Depositos/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDepositos(int id)
        {
            var depositos = await _context.Depositos.FindAsync(id);
            if (depositos == null)
            {
                return NotFound();
            }

            _context.Depositos.Remove(depositos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepositosExists(int id)
        {
            return _context.Depositos.Any(e => e.Id == id);
        }
    }
}
