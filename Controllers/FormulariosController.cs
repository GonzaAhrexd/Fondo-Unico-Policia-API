using FondoUnicoSistemaCompleto.Context;
using FondoUnicoSistemaCompleto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaFondoUnicoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [Authorize]
        public async Task<ActionResult<IEnumerable<Formularios>>> GetFormularios()
        {
            return await _context.Formularios.ToListAsync();
        }

        // GET: api/Formularios/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Formularios>> GetFormularios(int id)
        {
            var formularios = await _context.Formularios.FindAsync(id);

            if (formularios == null)
            {
                return NotFound();
            }

            return formularios;
        }
        // GET: api/Formularios/FiltrarHistorial/{tipo}/{desde}/{hasta}

        [HttpGet("FiltrarHistorial/{tipo}/{desde}/{hasta}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RegistroPreciosFormularios>>> FiltrarHistorial(
            string tipo, DateTime desde, DateTime hasta)
        {
            var registro = await _context.RegistroPreciosFormularios
                .Where(r => r.Formulario == tipo &&
                            r.desdeActivo.Date <= hasta.Date && // Empieza antes o en la fecha 'hasta' del filtro
                            (r.hastaActivo == null || r.hastaActivo.Value.Date >= desde.Date)) // Termina después o en la fecha 'desde' del filtro
                .OrderByDescending(r => r.Id)
                .FirstOrDefaultAsync();

            return Ok(registro);
        }


        // PUT: api/Formularios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PutFormularios(int id, Formularios formularios)
        {
            if (id != formularios.Id)
            {
                return BadRequest();
            }

            _context.Entry(formularios).State = EntityState.Modified;
            // Ahora en RegistroPreciosFormularios, busca el último registro del formulario y pon el hastaActivo en la fecha actual

            var registro = await _context.RegistroPreciosFormularios
                .Where(r => r.Formulario == formularios.Formulario)
                .OrderByDescending(r => r.desdeActivo)
                .FirstOrDefaultAsync();

            if (registro != null)
            {
                registro.hastaActivo = DateTime.Now; // Actualiza la fecha de finalización del último registro
                _context.Entry(registro).State = EntityState.Modified; // Marca el registro como modificado
            }

            // Ahora crea un nuevo registro en RegistroPreciosFormularios con el nuevo precio y la fecha actual
            var nuevoRegistro = new RegistroPreciosFormularios
            {
                desdeActivo = DateTime.Now,
                Formulario = formularios.Formulario,
                Importe = formularios.Importe,
                hastaActivo = null // Inicialmente no tiene fecha de finalización
            };
            _context.RegistroPreciosFormularios.Add(nuevoRegistro); // Añade el nuevo registro

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
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<Formularios>> PostFormularios(Formularios formularios)
        {
            _context.Formularios.Add(formularios);
            await _context.SaveChangesAsync(); // primero guarda el formulario

            var registroPrecio = new RegistroPreciosFormularios
            {
                desdeActivo = DateTime.Now,
                Formulario = formularios.Formulario, // debe tener valor
                Importe = formularios.Importe,
                hastaActivo = null
            };

            _context.RegistroPreciosFormularios.Add(registroPrecio);
            await _context.SaveChangesAsync(); // guarda el registro de precio

            return CreatedAtAction("GetFormularios", new { id = formularios.Id }, formularios);
        }

        // DELETE: api/Formularios/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
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
