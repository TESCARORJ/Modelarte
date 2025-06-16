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
    public class ObraRetrabalhoService : IObraRetrabalhoService
    {
        private readonly IObraRetrabalhoRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogAuditoriaRepository _logRepo;



        public ObraRetrabalhoService(IObraRetrabalhoRepository repo, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogAuditoriaRepository logRepo)
        {
            _repo = repo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logRepo = logRepo;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";


        public async Task<List<ObraRetrabalhoDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _repo.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraRetrabalhoDto>>(list);
        }

        public async Task CriarAsync(ObraRetrabalhoDto dto)
        {
            var entity = _mapper.Map<ObraRetrabalho>(dto); 
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            await _repo.AddAsync(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Criado",
                Descricao = $"Retrabalho '{entity.Descricao}' criado",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });
        }

        public async Task AtualizarAsync(ObraRetrabalhoDto dto)
        {
            var entityAntigo = await _repo.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<ObraRetrabalho>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            await _repo.UpdateAsync(entityNovo);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(ObraRetrabalho),
                Acao = "Atualizado",
                Descricao = $"Retrabalho '{entityNovo.Descricao}' atualizado",
                DadosAnteriores = JsonSerializer.Serialize(entityAntigo),
                DadosAtuais = JsonSerializer.Serialize(entityNovo)
            });
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
                await _repo.RemoveAsync(entity);
        }
    }
}
