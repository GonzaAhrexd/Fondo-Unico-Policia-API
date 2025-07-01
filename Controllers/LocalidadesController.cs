using FondoUnicoSistemaCompleto.Context;
using FondoUnicoSistemaCompleto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [Authorize] // Asegura que solo usuarios autenticados puedan acceder
        public async Task<ActionResult<IEnumerable<Localidades>>> GetLocalidades()
        {
            // Retorna todas las localidades desde la base de datos
            return await _context.Localidades.ToListAsync();
        }

        // GET: api/Localidades/5
        [HttpGet("{id}")]
        [Authorize] // Asegura que solo usuarios autenticados puedan acceder
        public async Task<ActionResult<Localidades>> GetLocalidades(int id)
        {
            // Busca una localidad específica por su ID
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
        [Authorize(Roles = "Administrador")] // Asegura que solo usuarios con el rol de Administrador puedan modificar localidades
        public async Task<IActionResult> PutLocalidades(int id, Localidades localidades)
        {
            // Verifica que el id uministrado coincida con el id de la localidad a modificar
            if(id != localidades.Id)
            {
                return BadRequest();
            }

            // Modifica la localidad en el contexto
            _context.Entry(localidades).State = EntityState.Modified;

            try
            {
                // Guarda los cambios en la base de datos
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
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<Localidades>> PostLocalidades(Localidades localidades)
        {
            // Agrega una nueva localidad al contexto
            _context.Localidades.Add(localidades);
            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocalidades", new { id = localidades.Id }, localidades);
        }

        // DELETE: api/Localidades/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteLocalidades(int id)
        {
            // Busca la localidad por su ID
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
