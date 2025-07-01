using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FondoUnicoSistemaCompleto.Context;
using FondoUnicoSistemaCompleto.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.CodeAnalysis.Host;

namespace FondoUnicoSistemaCompleto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly IConfiguration _configuration;

        public UsuariosController(IConfiguration configuration, ApplicationDBContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- Interfaz y Clases para Deserialización Optimizada ---
        // Se define una interfaz común para los datos de usuario que necesitamos
        

        // /api/usuarios/registrar-usuario
        [HttpPost("registrar-usuario")]
        [Authorize(Roles = "Administrador")] // Solo el administrador puede registrar usuarios
        public async Task<IActionResult> AltaUsuario([FromBody] UsuarioRequest request)
        {
            // Verifica que la solicitud no sea nula
            var urlPoliciaDigital = _configuration["POLICIA_DIGITAL_URL"];
            // Verifica que la URL de la Policía Digital esté configurada
            if(urlPoliciaDigital == null)
            {
                // Si la URL no está configurada, retorna un error 500
                return StatusCode(500, new { message = "La URL de la Policía Digital no está configurada." });
            }

            try
            {
                // Verifica que el request no sea nulo
                var usuarioExistente = await _context.Usuario.FirstOrDefaultAsync(u => u.DNI == request.Dni);
                if(usuarioExistente != null)
                {
                    return BadRequest(new { message = "El usuario ya existe" });
                }

                // Realiza la petición a la API externa de Policía Digital
                var response = await _httpClient.PostAsync(
                    $"{urlPoliciaDigital}/api_registroUsuario/usuario/find/usuarioSistema/{request.Dni}",
                    null
                );

                // Verifica si la respuesta fue exitosa
                var content = await response.Content.ReadAsStringAsync();

                IApiUserData apiUserData = null; // Usamos la interfaz común
                var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Deserializa la respuesta dependiendo del tipo de usuario
                if(request.tipoUsuario == "Civil")
                {
                    // Deserializa como CivilResponse
                    var usuarioResponse = JsonSerializer.Deserialize<UsuarioResponse>(content, serializerOptions);
                    if(usuarioResponse?.Data == null || usuarioResponse.Data.Civil == null)
                    {
                        return NotFound(new { message = "Usuario no encontrado" });
                    }
                    apiUserData = new CivilApiData
                    {
                        Id = usuarioResponse.Data.Id,
                        Usuario = usuarioResponse.Data.Usuario,
                        Civil = usuarioResponse.Data.Civil // Asigna el objeto Civil completo
                    };
                }
                else // Se asume que si no es Civil, es Policia (o cualquier otro tipo que siga PoliResponse)
                {
                    // Deserializa como PoliResponse
                    var poliResponse = JsonSerializer.Deserialize<PoliResponse>(content, serializerOptions);
                    if(poliResponse?.Data == null || poliResponse.Data.Persona == null) // También verifica Persona para Poli
                    {
                        return NotFound(new { message = "Usuario no encontrado" });
                    }
                    apiUserData = new PoliApiData
                    {
                        Id = poliResponse.Data.Id,
                        Usuario = poliResponse.Data.Usuario,
                        Persona = poliResponse.Data.Persona // Asigna el objeto Persona completo
                    };
                }

                // Si por alguna razón userData es nulo (ej. un tipoUsuario no manejado), manejarlo
                if(apiUserData == null)
                {
                    return BadRequest(new { message = "Tipo de usuario no válido o datos de API incompletos." });
                }

                // Lógica común de creación y guardado de usuario
                var altaUsuarioNuevo = new Usuario
                {
                    Rol = request.Rol,
                    Nombre = apiUserData.Nombre,
                    Apellido = apiUserData.Apellido,
                    Usuario_repo = apiUserData.Id.ToString(),
                    Nombre_de_usuario = apiUserData.Usuario,
                    DNI = request.Dni,
                };

                // Guarda el nuevo usuario en la base de datos 
                _context.Usuario.Add(altaUsuarioNuevo);
                _context.SaveChanges();

                // Retorna el usuario creado
                return Ok(altaUsuarioNuevo);
            }
            catch(JsonException jsonEx)
            {
                // Captura errores de deserialización JSON
                Console.WriteLine($"Error de deserialización JSON: {jsonEx.Message}");
                return BadRequest(new { message = "Formato de respuesta de la API externa inválido.", details = jsonEx.Message });
            }
            catch(HttpRequestException httpEx)
            {
                // Captura errores de la petición HTTP
                Console.WriteLine($"Error de petición HTTP a la API externa: {httpEx.Message}");
                return StatusCode(503, new { message = "No se pudo conectar con el servicio externo de Policía Digital.", details = httpEx.Message });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Ocurrió un error inesperado al registrar el usuario.", details = ex.Message });
            }
        }

        // //api/usuarios/buscar-usuario-dni/{dni}
        [HttpGet("buscar-usuario-dni/{dni}")]
        [Authorize(Roles = "Administrador")] // Solo el administrador puede buscar usuarios por DNI
        public async Task<IActionResult> BuscarUsuarioDNI(string dni)
        {
            try
            {
                // Verifica que el DNI no sea nulo o vacío
                var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.DNI == dni);
                if(usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
                // Retorna el usuario encontrado
                return Ok(usuario);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Error interno del servidor al buscar usuario por DNI.", details = ex.Message });
            }
        }
        // api/usuarios/buscar-usuarios/{rol}
        [HttpGet("buscar-usuarios/{rol}")]
        [Authorize(Roles = "Administrador")] // Solo el administrador puede buscar usuarios por rol
        public async Task<IActionResult> BuscarUsuarios(string rol)
        {
            try
            {
                // Verifica que el rol no sea nulo o vacío
                bool rolIngresado = rol != "no_ingresado";
                // Si el rol no fue ingresado, se buscarán todos los usuarios
                var query = _context.Usuario.AsQueryable();
                // Si se ingresó un rol, filtra por ese rol
                if(rolIngresado)
                    query = query.Where(u => u.Rol == rol);

                // Ejecuta la consulta y obtiene la lista de usuarios
                return Ok(await query.ToListAsync());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Error interno del servidor al buscar usuarios por rol.", details = ex.Message });
            }
        }

        // api/usuarios/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Verifica que la URL de la Policía Digital esté configurada
            var urlPoliciaDigital = _configuration["POLICIA_DIGITAL_URL"];
            
            // Verifica que la clave secreta del token esté configurada
            var secretKey = _configuration["SECRET_TOKEN_KEY"];
            if(secretKey == null)
            {
                return StatusCode(500, new { message = "La clave secreta del token no está configurada." });
            }

            try
            {
                // Crea el cuerpo de la solicitud para Policía Digital
                var requestBody = new
                {
                    usuario = request.Usuario,
                    clave = request.Clave
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                // Realiza la petición POST a la API externa de Policía Digital
                var response = await _httpClient.PostAsync(
                    $"{urlPoliciaDigital}/api_registroUsuario/usuario/find/loginSistemas", jsonContent
                );

                // Guarda el contenido de la respuesta
                var contentString = await response.Content.ReadAsStringAsync();

                // Verificar si la respuesta de la API externa fue exitosa
                if(!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { message = "Error al autenticar con el servicio externo.", details = contentString });
                }

                // Deserializa la respuesta JSON de la API externa
                var contentObject = JsonSerializer.Deserialize<ResponseModel>(contentString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Asumiendo que `data` en ResponseModel es el ID del repositorio
                int idRepo = contentObject.data;

                var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Usuario_repo == idRepo.ToString());

                if(usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado en el sistema interno." });
                }

                // Generamos un token si el usuario existe
                var tokenHandler = new JwtSecurityTokenHandler();
                var byteKey = Encoding.UTF8.GetBytes(secretKey);
                var tokenDes = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, usuario.Nombre),
                        new Claim(ClaimTypes.Role, usuario.Rol),
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(24),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(byteKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDes);
                string tokenString = tokenHandler.WriteToken(token);

                Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
                {
                    HttpOnly = false, // Considera true si no necesitas acceder desde JavaScript en el cliente
                    Secure = false,   // Debería ser true en producción (HTTPS)
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(24)
                });

                return Ok(new { usuario, tokenString });
            }
            catch(JsonException jsonEx)
            {
                Console.WriteLine($"Error de deserialización JSON en Login: {jsonEx.Message}");
                return BadRequest(new { message = "Formato de respuesta de la API externa inválido al iniciar sesión.", details = jsonEx.Message });
            }
            catch(HttpRequestException httpEx)
            {
                Console.WriteLine($"Error de petición HTTP a la API externa en Login: {httpEx.Message}");
                return StatusCode(503, new { message = "No se pudo conectar con el servicio externo de Policía Digital para el login.", details = httpEx.Message });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Ocurrió un error inesperado al intentar iniciar sesión.", details = ex.Message });
            }
        }

        // api/usuarios/logged
        [HttpGet("logged")]
        [Authorize]
        public async Task<IActionResult> GetUserLogged()
        {
            try
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if(idClaim == null || !int.TryParse(idClaim.Value, out int id))
                {
                    return Unauthorized(new { message = "ID de usuario no encontrado o inválido en el token." });
                }

                var usuario = await _context.Usuario.FindAsync(id);

                if(usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado en la base de datos interna." });
                }
                return Ok(usuario);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Ocurrió un error inesperado al obtener el usuario logueado.", details = ex.Message });
            }
        }

        // api/usuarios/delete-usuario/{id}
        [HttpDelete("delete-usuario/{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                // Verifica que el ID del usuario no sea nulo o inválido
                var usuario = await _context.Usuario.FindAsync(id);
                if(usuario == null)
                {
                    return NotFound(new { message = "Usuario a eliminar no encontrado." });
                }
                // Elimina el usuario de la base de datos
                _context.Usuario.Remove(usuario);
                await _context.SaveChangesAsync();

                return NoContent(); // 204 No Content para eliminación exitosa
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Ocurrió un error inesperado al eliminar el usuario.", details = ex.Message });
            }
        }

        // api/usuarios/update-usuario/{id}
        [HttpPut("update-usuario/{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioRequestEdit request)
        {
            try
            {
                // Verifica que el ID del usuario no sea nulo o inválido
                var usuario = await _context.Usuario.FindAsync(id);
                if(usuario == null)
                {
                    return NotFound(new { message = "Usuario a actualizar no encontrado." });
                }

                // Solo se permite actualizar el rol
                usuario.Rol = request.Rol;

                // Actualiza el usuario en la base de datos
                await _context.SaveChangesAsync();

                return Ok(usuario);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Ocurrió un error inesperado al actualizar el usuario.", details = ex.Message });
            }
        }

        /* --- Clases de modelos (Data Transfer Objects - DTOs) ---
           Se mantienen las clases originales, ya que representan la estructura de las respuestas JSON de la API externa
            y los requests de tu API.
        */ 

        public class LoginRequest
        {
            public string Usuario { get; set; }
            public string Clave { get; set; }
        }

        public class ResponseModel
        {
            public string code { get; set; }
            public string msg { get; set; }
            public int data { get; set; } // Asumiendo que 'data' es un entero (ID) para el login
            public bool anonimo { get; set; }
        }

        public class UsuarioRequestEdit
        {
            public string Rol { get; set; }
        }

        public interface IApiUserData
        {
            string Nombre { get; }
            string Apellido { get; }
            string Usuario { get; }
            int Id { get; }
        }

        // Clase auxiliar para mapear los datos de Civil
        public class CivilApiData : IApiUserData
        {
            public int Id { get; set; }
            public string Usuario { get; set; }
            public CivilData Civil { get; set; } // Referencia al objeto CivilData real

            // Implementación explícita de la interfaz
            string IApiUserData.Nombre => Civil?.Nombre;
            string IApiUserData.Apellido => Civil?.Apellido;
            string IApiUserData.Usuario => Usuario;
            int IApiUserData.Id => Id;
        }

        // Clase auxiliar para mapear los datos de Policia
        public class PoliApiData : IApiUserData
        {
            public int Id { get; set; }
            public string Usuario { get; set; }
            public PersonaData Persona { get; set; } // Referencia al objeto PersonaData real

            // Implementación explícita de la interfaz
            string IApiUserData.Nombre => Persona?.Nombre;
            string IApiUserData.Apellido => Persona?.Apellido;
            string IApiUserData.Usuario => Usuario;
            int IApiUserData.Id => Id;
        }
       
       

        public class UsuarioRequest
        {
            public string Dni { get; set; }
            public string Rol { get; set; }
            public string tipoUsuario { get; set; }
        }

        /* Estas clases representan las respuestas exactas de la API externa.
         Las interfaces IApiUserData y las clases CivilApiData/PoliApiData se encargan de
         mapear estas respuestas a un formato común para tu aplicación.
        */

        public class UsuarioResponse
        {
            public string Code { get; set; }
            public string Msg { get; set; }
            public UsuarioData Data { get; set; }
        }

        public class UsuarioData
        {
            public int Id { get; set; }
            public string Usuario { get; set; }
            public RolData Rol { get; set; }
            public bool Policia { get; set; }
            public object Persona { get; set; } // Puede ser `null` para civiles
            public CivilData Civil { get; set; }
        }

        public class PoliResponse
        {
            public string Code { get; set; }
            public string Msg { get; set; }
            public PoliUsuarioData Data { get; set; }
        }

        public class PoliUsuarioData
        {
            public int Id { get; set; }
            public string Usuario { get; set; }
            public RolData Rol { get; set; }
            public bool Policia { get; set; }
            public PersonaData Persona { get; set; } // Datos del policía
            public object Civil { get; set; } // En este caso, `null`
        }

        // Las siguientes clases son sub-objetos de las respuestas de la API externa
        public class RolData
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        public class CivilData
        {
            public int Id { get; set; }
            public int Sexo { get; set; }
            public string Apellido { get; set; }
            public string Nombre { get; set; }
            public string NorDni { get; set; }
            public string GrupoS { get; set; }
            public string Factor { get; set; }
            public string FechaNacimiento { get; set; }
            public string Domicilio { get; set; }
            public string FechaFinContrato { get; set; }
            public int Activo { get; set; }
            public int Unidad { get; set; }
            public int UsuarioCrea { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class PersonaData
        {
            public int Id { get; set; }
            public SexoData Sexo { get; set; }
            public int TipoDocumento { get; set; }
            public string Apellido { get; set; }
            public string Nombre { get; set; }
            public string NorDni { get; set; }
            public string Email { get; set; }
            public string NroTelefono { get; set; }
            public string GrupoS { get; set; }
            public string Factor { get; set; }
            public string FechaNacimiento { get; set; }
            public int NroLegajo { get; set; }
            public string AnoIngresoPolicia { get; set; }
            public int AnosServicio { get; set; }
            public int MesesServicio { get; set; }
            public int DiasServicio { get; set; }
            public int AnosOtroServicio { get; set; }
            public int MesesOtroServicio { get; set; }
            public int DiasOtroServicio { get; set; }
            public int Activo { get; set; }
            public int EstadoJunta { get; set; }
            public string ArmaNombre { get; set; }
            public string ArmaNro { get; set; }
        }

        public class SexoData
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public int Activo { get; set; }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }
    }
}