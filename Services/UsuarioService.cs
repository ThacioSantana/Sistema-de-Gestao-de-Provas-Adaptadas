using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SistemaDeChamadosNAA.Services;

public class UsuarioService : IUsuarioService
{
    public string GetPerfilUsuario(HttpContext context)
    {
        return context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
    }

    public int GetUsuarioId(HttpContext context)
    {
        var userIdClaim = context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }
}