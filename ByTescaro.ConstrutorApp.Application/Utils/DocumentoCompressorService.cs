using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Utils
{
    public static class DocumentoCompressorService
    {
        private const long MaxTamanhoKb = 500;

        public static async Task<MemoryStream> ComprimirSeNecessarioAsync(IBrowserFile file)
        {
            if (file.Size <= MaxTamanhoKb * 1024)
                return await CopiarArquivoAsync(file);

            var extensao = Path.GetExtension(file.Name).ToLowerInvariant();
            return extensao switch
            {
                ".pdf" => await CompactarPdfAsync(file),
                ".docx" or ".xlsx" or ".pptx" => await CompactarZipadoAsync(file),
                _ => await CopiarArquivoAsync(file)
            };
        }

        private static async Task<MemoryStream> CopiarArquivoAsync(IBrowserFile file)
        {
            var memory = new MemoryStream();
            await using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // Aumenta o limite para 10 MB
            await stream.CopyToAsync(memory);
            memory.Position = 0;
            return memory;
        }

        private static async Task<MemoryStream> CompactarZipadoAsync(IBrowserFile file)
        {
            // Cria um novo ZIP com o arquivo dentro (simples compressão)
            var memory = new MemoryStream();
            using var archive = new ZipArchive(memory, ZipArchiveMode.Create, leaveOpen: true);
            var zipEntry = archive.CreateEntry(file.Name, CompressionLevel.Fastest);

            await using var zipStream = zipEntry.Open();
            await using var fileStream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            await fileStream.CopyToAsync(zipStream);

            memory.Position = 0;
            return memory;
        }

        private static async Task<MemoryStream> CompactarPdfAsync(IBrowserFile file)
        {
            // 📌 Simulação de compressão apenas (regravação em memória)
            // Pode ser substituído por iText7, PDFSharp ou outro se for viável
            var memory = new MemoryStream();
            await using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            await stream.CopyToAsync(memory);
            memory.Position = 0;

            // Opcional: integrar bibliotecas como PDFSharp com redução real.
            return memory;
        }
    }
}
