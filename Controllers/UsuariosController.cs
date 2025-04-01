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

        // Haz una ruta /registrar-usuario 
        [HttpPost("registrar-usuario")]
        public async Task<IActionResult> AltaUsuario([FromBody] UsuarioRequest request){
            // Obtener del archivo .env POLICIA_DIGITAL_URL

        
               var urlPoliciaDigital = _configuration["POLICIA_DIGITAL_URL"];



            if (urlPoliciaDigital == null)
            {
                return StatusCode(500, urlPoliciaDigital);
            }


            // Extra del body solamente dni
            try
            {
                    // Busca si el DNI ya existe en la BD
                    var usuarioExistente = await _context.Usuario.FirstOrDefaultAsync(u => u.DNI == request.Dni);
                if(usuarioExistente != null)
                {
                    
                        return BadRequest(new { message = "El usuario ya existe" });
                    }

                
                var response = await _httpClient.PostAsync(
                    $"{urlPoliciaDigital}/api_registroUsuario/usuario/find/usuarioSistema/{request.Dni}",
                    null
                );

                var content = await response.Content.ReadAsStringAsync();




                if(request.tipoUsuario == "Civil"){

                // Deserializar JSON a objeto
                var usuario = JsonSerializer.Deserialize<UsuarioResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });


                if(usuario?.Data == null || usuario?.Data.Civil == null)
                    {

                    return NotFound(new { message = "Usuario no encontrado" });
                    }



                    
                  var AltaUsuarioNuevo = new Usuario
                  {
                      Rol = request.Rol,
                      Nombre = usuario.Data.Civil.Nombre,
                      Apellido =  usuario.Data.Civil.Apellido,
                      Usuario_repo =  usuario.Data.Id.ToString(),
                      Nombre_de_usuario =  usuario.Data.Usuario,
                      DNI = request.Dni, 
                  };

                  _context.Usuario.Add(AltaUsuarioNuevo);

                  _context.SaveChanges();

                    return Ok(AltaUsuarioNuevo);

                }
                else
                {
                    var usuario = JsonSerializer.Deserialize<PoliResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if(usuario?.Data == null)
                        return NotFound(new { message = "Usuario no encontrado" });



                    var AltaUsuarioNuevo = new Usuario
                    {
                        Rol = request.Rol,
                        Nombre = usuario.Data.Persona.Nombre,
                        Apellido =  usuario.Data.Persona.Apellido,
                        Usuario_repo =  usuario.Data.Id.ToString(),
                        Nombre_de_usuario =  usuario.Data.Usuario,
                        DNI = request.Dni,
                    };


                    _context.Usuario.Add(AltaUsuarioNuevo);
                    _context.SaveChanges();

                    

                    return Ok(AltaUsuarioNuevo);

                }
              
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", details = ex.ToString() });

            }
    
            }

        [HttpGet("buscar-usuario-dni/{dni}")]
        [Authorize]
        public async Task<IActionResult> BuscarUsuarioDNI(string dni){
            try{
                
                var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.DNI == dni);
                if(usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(usuario);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Error interno del servidor");   
            }



        }   

        [HttpGet("buscar-usuarios/{rol}")]
        [Authorize]
        public async Task<IActionResult> BuscarUsuarios(string rol )
        {
            try
            {
                bool rolIngresado = rol != "no_ingresado";

                var query = _context.Usuario.AsQueryable();

                if(rolIngresado)
                    query = query.Where(u => u.Rol == rol);

           


                return Ok(await query.ToListAsync());

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
//            var urlPoliciaDigital = Environment.GetEnvironmentVariable("POLICIA_DIGITAL_URL");

            var urlPoliciaDigital = _configuration["POLICIA_DIGITAL_URL"];
            var secretKey = _configuration["SECRET_TOKEN_KEY"];


            if(secretKey == null)
            {
                return StatusCode(500, secretKey);
            }

            //return Ok(secretKey);
            try
            {

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

                var response = await _httpClient.PostAsync(
                    $"{urlPoliciaDigital}/api_registroUsuario/usuario/find/loginSistemas", jsonContent
                );

                // Tengo que acceder a response.data.data
                var contentString = await response.Content.ReadAsStringAsync();

                // Define una clase para deserializar el JSON
                var contentObject = JsonSerializer.Deserialize<ResponseModel>(contentString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Ignora mayúsculas/minúsculas en las propiedades
                });
                // Extrae data y guardalo en Id sesión
                int idRepo = contentObject.data;

                // Ahora buscalo en la base de datos
                var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Usuario_repo == idRepo.ToString());



                if(usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // Si el usuario existe, generamos un token
                var tokenHandler = new JwtSecurityTokenHandler();
                var byteKey = Encoding.UTF8.GetBytes(secretKey);
                var tokenDes = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, usuario.Nombre),
                        new Claim(ClaimTypes.Role, usuario.Rol),
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
                        //  new Claim(ClaimTypes.)
                    }),

                    Expires = DateTime.UtcNow.AddHours(24),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(byteKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDes);


                string tokenString = tokenHandler.WriteToken(token);

                Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
                {
                    HttpOnly = false,       // Hace que el token no sea accesible desde JavaScript
                    Secure = false,         // Asegura la cookie en conexiones HTTPS
                    SameSite = SameSiteMode.Strict, // Previene que la cookie sea enviada en solicitudes cross-site
                    Expires = DateTime.UtcNow.AddHours(24) // Configura el tiempo de expiración
                });

                // Retorna los datos del usuario y la cookie por separado
                return Ok(new { usuario, tokenString });


                //return Ok(tokenString


            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, ex);
            }
        }

        [HttpGet("logged")]
        [Authorize]
        //Recupera los datos del usuario con el token que viene en el header
        public async Task<IActionResult> getUserLogged()
        {
            var id = int.Parse( User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuario = await _context.Usuario.FindAsync(id);

            if(usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(usuario);
        }



        public class LoginRequest
        {
            public string Usuario { get; set; }
            public string Clave { get; set; }
        }

        public class ResponseModel
        {
            public string code { get; set; }
            public string msg { get; set; }
            public int data { get; set; }
            public bool anonimo { get; set; }
        }


        // Delete user
        [HttpDelete("delete-usuario/{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
        
            
          var usuario = await _context.Usuario.FindAsync(id);

 
            if (usuario == null)
            {
                return NotFound();
            }
           
    

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        
            }

        [HttpPut("update-usuario/{id}")]
       // Necesito que solo tome Rol y Unidad y solo modifique eso de la BD
       public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioRequestEdit request)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Rol = request.Rol;

            await _context.SaveChangesAsync();

            return Ok(usuario);
        }




        private string GenerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ClaveSuperSecreta"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new Claim(ClaimTypes.Name, usuario.Usuario_repo)
    };

            var token = new JwtSecurityToken(
                issuer: "policia-digital",
                audience: "tu-app",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class UsuarioRequestEdit{ 

            public string Rol { get; set; }
        }

        public class UsuarioRequest
        {
            public string Dni { get; set; }
            public string Rol { get; set; }
            public string tipoUsuario { get; set; }
        }

        public class UsuarioPoliciaResponse {          

        public string Code { get; set; }
        public string Msg { get; set; }
        public UsuarioPoliciaData Data { get; set; }


    }


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
            public object Persona { get; set; } // Puede ser `null`, así que lo dejamos como `object`
            public CivilData Civil { get; set; }
        }
        public class UsuarioPoliciaData
        {
            public int Id { get; set; }
            public string Usuario { get; set; }
            public RolData Rol { get; set; }
            public bool Policia { get; set; }
            public CivilData Persona { get; set; } // Puede ser `null`, así que lo dejamos como `object`
            public object Civil { get; set; }
        }


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
