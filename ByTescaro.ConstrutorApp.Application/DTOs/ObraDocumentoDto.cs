using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraDocumentoDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public string NomeOriginal { get; set; } = string.Empty;
        public string CaminhoRelativo { get; set; } = string.Empty;
        public string Extensao { get; set; } = string.Empty;
        public long TamanhoEmKb { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
    }
}

