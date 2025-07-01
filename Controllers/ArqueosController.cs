using FondoUnicoSistemaCompleto.Context;
using FondoUnicoSistemaCompleto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        [Authorize]
        public async Task<ActionResult<IEnumerable<Arqueos>>> GetArqueos()
        {
            return await _context.Arqueos.ToListAsync();
        }

        // GET: api/Arqueos/5
        [HttpGet("{id}")]
        [Authorize]
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
        [Authorize]
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

        // Dentro de un controlador, después de que el usuario esté autenticado
        [HttpGet("mi-info")]
        [Authorize(Roles = "Administrador")] // Asegura que haya un usuario autenticado
        public IActionResult GetMyInfo()
        {
            // Acceder al ClaimsPrincipal del usuario actual
            var currentUser = HttpContext.User;

            // Obtener claims específicos:

            // ID del usuario (ClaimTypes.NameIdentifier es un claim común para esto)
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Nombre del usuario (ClaimTypes.Name o "name")
            var userName = currentUser.FindFirst(ClaimTypes.Name)?.Value;

            // Email del usuario (ClaimTypes.Email o "email")
            var userEmail = currentUser.FindFirst(ClaimTypes.Email)?.Value;

            // Roles del usuario (ClaimTypes.Role o "role")
            var userRoles = currentUser.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            // Puedes iterar sobre todos los claims para ver qué hay disponible
            var allClaims = currentUser.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();

            return Ok(new
            {
                UserId = userId,
                UserName = userName,
                UserEmail = userEmail,
                UserRoles = userRoles,
                AllClaims = allClaims // Para depuración, muestra todo
            });
        }

        // POST: api/Arqueos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
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
        [Authorize]
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
