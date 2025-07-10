namespace SistemaDeChamadosNAA.Services;

public interface IUsuarioService
{
    string GetPerfilUsuario(HttpContext context);
    int GetUsuarioId(HttpContext context);
}