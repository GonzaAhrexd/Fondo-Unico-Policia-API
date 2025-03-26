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

namespace FondoUnicoSistemaCompleto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntregasController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public EntregasController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Entregas
        [HttpGet]
       // [Authorize]
        public async Task<ActionResult<IEnumerable<Entregas>>> GetEntregas()
        {
           return await _context.Entregas.Where(e => e.estaActivo == true ).Include(e => e.RenglonesEntregas).ToListAsync();
           
        }

        // GET: api/Entregas/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Entregas>> GetEntregas(int id)
        {
            var entregas = await _context.Entregas.FindAsync(id);
            // Haz que traiga la entrega con sus renglones
            entregas = await _context.Entregas.Include(e => e.RenglonesEntregas).FirstOrDefaultAsync(e => e.NroEntregaManual == id && e.estaActivo == true);
            if (entregas == null)
            {
                return NotFound();
            }

            return entregas;
        }
        // Agrega una petición que sea del formato api/Entregas/Unidad/FechaInicio/FechaFinal, es decir que se pueda pasar Unidad, FechaInicio y FechaFinal en los params

        [HttpGet("{unidad}/{fechaInicio}/{fechaFinal}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Entregas>>> GetEntregasPorUnidadFecha(string unidad, DateTime fechaInicio, DateTime fechaFinal)
        {

            // Si Unidad es Listar Todo, no filtrar por Unidad
            if(unidad == "Listar todo")
            {
                return await _context.Entregas.Include(e => e.RenglonesEntregas).Where(e => e.Fecha >= fechaInicio && e.Fecha <= fechaFinal && e.estaActivo == true).ToListAsync();
            }

            return await _context.Entregas.Include(e => e.RenglonesEntregas).Where(e => e.Unidad == unidad && e.Fecha >= fechaInicio && e.Fecha <= fechaFinal && e.estaActivo == true).ToListAsync();
        }


        // PUT: api/Entregas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutEntregas(int id, Entregas entregas)
        {
            entregas.estaActivo = true;
            if(id != entregas.NroEntrega)
            {
                return BadRequest();
            }

            _context.Entry(entregas).State = EntityState.Modified;

            // Actualizar RenglonesEntregas
            foreach(var renglon in entregas.RenglonesEntregas)
            {
                if(renglon.Id == 0)
                {
                    // Si el Id es 0, es un nuevo renglon
                    _context.RenglonesEntregas.Add(renglon);
                }
                else
                {
                    // Si el Id no es 0, es un renglon existente
                    _context.Entry(renglon).State = EntityState.Modified;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!EntregasExists(id))
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


        // POST: api/Entregas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<Entregas>> PostEntregas(Entregas entregas)
        {
            entregas.estaActivo = true;
            _context.Entregas.Add(entregas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntregas", new { id = entregas.NroEntrega }, entregas);
        }

        // DELETE: api/Entregas/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEntregas(int id)
        {
            var entrega = await _context.Entregas.Include(e => e.RenglonesEntregas).FirstOrDefaultAsync(e => e.NroEntrega == id);

            if(entrega == null)
            {
                return NotFound();
            }

            // Eliminar renglones asociados si existen
            if(entrega.RenglonesEntregas != null)
            {
                _context.RenglonesEntregas.RemoveRange(entrega.RenglonesEntregas);
            }

            // Marcar la entrega como inactiva
            entrega.estaActivo = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool EntregasExists(int id)
        {
            return _context.Entregas.Any(e => e.NroEntrega == id);
        }
    }
}
