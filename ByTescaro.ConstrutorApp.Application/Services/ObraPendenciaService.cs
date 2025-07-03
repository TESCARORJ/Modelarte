using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Interfaces.ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraPendenciaService : IObraPendenciaService
    {
        private readonly IObraPendenciaRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogAuditoriaRepository _logRepo;



        public ObraPendenciaService(IObraPendenciaRepository repo, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogAuditoriaRepository logRepo)
        {
            _repo = repo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logRepo = logRepo;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";


        public async Task<List<ObraPendenciaDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _repo.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraPendenciaDto>>(list);
        }

        public async Task CriarAsync(ObraPendenciaDto dto)
        {
            var entity = _mapper.Map<ObraPendencia>(dto); 
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            _repo.Add(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Criado",
                Descricao = $"Pendência '{entity.Descricao}' criado",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });
        }

        public async Task AtualizarAsync(ObraPendenciaDto dto)
        {
            var entityAntigo = await _repo.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<ObraPendencia>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            _repo.Remove(entityNovo);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(ObraPendencia),
                Acao = "Atualizado",
                Descricao = $"Pendência '{entityNovo.Descricao}' atualizado",
                DadosAnteriores = JsonSerializer.Serialize(entityAntigo),
                DadosAtuais = JsonSerializer.Serialize(entityNovo)
            });
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
                _repo.Remove(entity);
        }
    }
}
