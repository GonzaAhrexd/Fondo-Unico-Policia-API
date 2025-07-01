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
    public class BancosController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public BancosController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Bancos
        [HttpGet]
        [Authorize] // Asegúrate de que el usuario esté autenticado
        public async Task<ActionResult<IEnumerable<Bancos>>> GetBancos()
        {
            // Regresa todos los bancos
            return await _context.Bancos.ToListAsync();
        }

        // GET: api/Bancos/5
        [HttpGet("{id}")]
        [Authorize] // Asegúrate de que el usuario esté autenticado
        public async Task<ActionResult<Bancos>> GetBancos(int id)
        {
            // Busca un banco por su ID
            var bancos = await _context.Bancos.FindAsync(id);

            // Si no se encuentra el banco, retorna NotFound
            if(bancos == null)
            {
                return NotFound();
            }
            // Si se encuentra, retorna el banco
            return bancos;
        }

        // PUT: api/Bancos/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")] // Asegúrate de que el usuario tenga el rol de Administrador
        public async Task<IActionResult> PutBancos(int id, Bancos bancos)
        {
            // Asegúrate de que el banco esté activo
            if(id != bancos.Id)
            {
                return BadRequest();
            }

            // Cambia el estado del banco a modificado
            _context.Entry(bancos).State = EntityState.Modified;

            try{
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException){
                if(!BancosExists(id)){
                    return NotFound();
                }
                else{
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Bancos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<Bancos>> PostBancos(Bancos bancos)
        {
            // Agrega un nuevo banco a la base de datos
            _context.Bancos.Add(bancos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBancos", new { id = bancos.Id }, bancos);
        }

        // DELETE: api/Bancos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteBancos(int id)
        {
            // Busca el banco por su ID
            var bancos = await _context.Bancos.FindAsync(id);
            if(bancos == null)
            {
                return NotFound();
            }

            // Elimina el banco de la base de datos
            _context.Bancos.Remove(bancos);
            // Guarda los cambios
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BancosExists(int id)
        {
            return _context.Bancos.Any(e => e.Id == id);
        }
    }
}
