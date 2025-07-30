using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraFuncionarioRelatorioDto
    {
        public long ObraId { get; set; }

        public long FuncionarioId { get; set; }
        public string? FuncionarioNome { get; set; } = string.Empty;

        public string? FuncaoNoObra { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }
}
