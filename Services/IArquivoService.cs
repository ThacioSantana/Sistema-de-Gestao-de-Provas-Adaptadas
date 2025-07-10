namespace SistemaDeChamadosNAA.Services;

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface IArquivoService
{
    Task<string> SalvarArquivoAsync(IFormFile arquivo, string subpasta);
}