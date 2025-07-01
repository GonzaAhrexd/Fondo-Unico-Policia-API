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
    public class UnidadesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UnidadesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Unidades
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Unidades>>> GetUnidades()
        {
            var ordinales = new Dictionary<string, int>
    {
        { "Primera", 1 }, { "Segunda", 2 }, { "Tercera", 3 }, { "Cuarta", 4 },
        { "Quinta", 5 }, { "Sexta", 6 }, { "Séptima", 7 }, { "Octava", 8 },
        { "Novena", 9 }, { "Décima", 10 }, { "Undécima", 11}, { "Duodécima", 12}, { "Decimotercera", 13},
        { "Decimocuarta", 14}, { "Decimoquinta", 15}, { "Decimosexta", 16}, { "Decimoséptima", 17},
        { "Decimoctava", 18}, { "Decimonovena", 19}, { "Vigésima" , 20 }
    };

            var unidades = await _context.Unidades.ToListAsync(); // Trae los datos de la BD

            return unidades
                .OrderBy(x =>
                {
                    if(string.IsNullOrWhiteSpace(x.Unidad))
                        return "99"; // Si la unidad está vacía, la manda al final

                    var palabras = x.Unidad.Split(' '); // Dividimos la cadena en palabras
                    var ultimaPalabra = palabras.Last(); // Última palabra (Ej: Resistencia, Fontana)

                    // Buscar el número ordinal en la frase
                    var numeroOrdinal = palabras.FirstOrDefault(p => ordinales.ContainsKey(p));

                    // Si hay número ordinal, convertirlo a número, si no, usar "99" para que vaya al final
                    var numeroOrden = numeroOrdinal != null ? ordinales[numeroOrdinal].ToString("D2") : "99";

                    // Primero ordenar por la última palabra (alfabéticamente)
                    // Luego ordenar por el número ordinal
                    return ultimaPalabra + "_" + numeroOrden;
                })
                .ToList();
        }
        [HttpGet("Verificaciones")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetUnidadesVerificaciones()
        {
            var unidades = await _context.Unidades
                .Where(u => u.Verificaciones == true)
                .Select(u => new { u.Id, u.Unidad })
                .ToListAsync();

            return Ok(unidades);
        }



        // GET: api/Unidades/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Unidades>> GetUnidades(int id)
        {
            var unidades = await _context.Unidades.FindAsync(id);

            if(unidades == null)
            {
                return NotFound();
            }

            return unidades;
        }

        // PUT: api/Unidades/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PutUnidades(int id, Unidades unidades)
        {
            if(id != unidades.Id)
            {
                return BadRequest();
            }

            _context.Entry(unidades).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!UnidadesExists(id))
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
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<Unidades>> PostUnidades(Unidades unidades)
        {
            _context.Unidades.Add(unidades);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUnidades", new { id = unidades.Id }, unidades);
        }

        // DELETE: api/Unidades/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteUnidades(int id)
        {
            var unidades = await _context.Unidades.FindAsync(id);
            if(unidades == null)
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
