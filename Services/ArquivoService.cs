namespace SistemaDeChamadosNAA.Services;

using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Threading.Tasks;

public class ArquivoService : IArquivoService
{
    public async Task<string> SalvarArquivoAsync(IFormFile arquivo, string subpasta)
    {
        if (arquivo == null || arquivo.Length == 0)
            throw new ArgumentException("Arquivo inválido.");

        var pastaBase = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", subpasta);
        Directory.CreateDirectory(pastaBase); // Garante que a pasta exista

        var nomeArquivo = $"{Guid.NewGuid()}_{arquivo.FileName}";
        var caminhoCompleto = Path.Combine(pastaBase, nomeArquivo);

        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
        {
            await arquivo.CopyToAsync(stream);
        }

        return caminhoCompleto;
    }
}