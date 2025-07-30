using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraImagemRelatorioDto
    {
        public long ObraId { get; set; }
        public string? NomeOriginal { get; set; } = string.Empty;
        public string? CaminhoRelativo { get; set; } = string.Empty;
        public long TamanhoEmKb { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }
}
