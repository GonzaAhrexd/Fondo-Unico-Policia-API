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
    public class RegistroEntregasController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public RegistroEntregasController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/RegistroEntregas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistroEntregas>>> GetRegistroEntregas()
        {
            return await _context.RegistroEntregas.ToListAsync();
        }

        // GET: api/RegistroEntregas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegistroEntregas>> GetRegistroEntregas(int id)
        {
            var registroEntregas = await _context.RegistroEntregas.FindAsync(id);

            if (registroEntregas == null)
            {
                return NotFound();
            }

            return registroEntregas;
        }

        // PUT: api/RegistroEntregas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegistroEntregas(int id, RegistroEntregas registroEntregas)
        {
            if (id != registroEntregas.Id)
            {
                return BadRequest();
            }

            _context.Entry(registroEntregas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegistroEntregasExists(id))
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

        // POST: api/RegistroEntregas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RegistroEntregas>> PostRegistroEntregas(RegistroEntregas registroEntregas)
        {
            _context.RegistroEntregas.Add(registroEntregas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegistroEntregas", new { id = registroEntregas.Id }, registroEntregas);
        }

        // DELETE: api/RegistroEntregas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistroEntregas(int id)
        {
            var registroEntregas = await _context.RegistroEntregas.FindAsync(id);
            if (registroEntregas == null)
            {
                return NotFound();
            }

            _context.RegistroEntregas.Remove(registroEntregas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Devuelve el último registro de entrega hasta una fecha ingresada y toma el valor de la cantidad actual y devuelvela 
        // GET: api/RegistroEntregas/CantidadActual/fecha

        [HttpGet("CantidadActual/{fecha}/{Unidad}/{tipoEntrega}")]
        public async Task<ActionResult<int>> GetCantidadActual(string fecha, string unidad, string tipoEntrega)
        {
            DateTime fechaConsulta = DateTime.Parse(fecha).Date.AddDays(1).AddTicks(-1);

            var registroEntregas = await _context.RegistroEntregas
                .Where(r => r.Fecha <= fechaConsulta
                            && r.Unidad == unidad
                            && r.TipoEntrega == tipoEntrega)
                .OrderByDescending(r => r.Fecha)
                .FirstOrDefaultAsync();


            if(registroEntregas == null)
            {
                return 0;
            }

            return registroEntregas.cantidadActual;
        }   
        private bool RegistroEntregasExists(int id)
        {
            return _context.RegistroEntregas.Any(e => e.Id == id);
        }
    }
}
