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
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IUnitOfWork _unitOfWork;




        public ObraPendenciaService(IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogAuditoriaRepository logRepo, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logRepo = logRepo;
            _unitOfWork = unitOfWork;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";


        public async Task<List<ObraPendenciaDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraPendenciaRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraPendenciaDto>>(list);
        }

        public async Task CriarAsync(ObraPendenciaDto dto)
        {
            var entity = _mapper.Map<ObraPendencia>(dto); 
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            _unitOfWork.ObraPendenciaRepository.Add(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Criado",
                Descricao = $"Pendência '{entity.Descricao}' criado",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });

            await _unitOfWork.CommitAsync();

        }

        public async Task AtualizarAsync(ObraPendenciaDto dto)
        {
            var entityAntigo = await _unitOfWork.ObraPendenciaRepository.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<ObraPendencia>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            _unitOfWork.ObraPendenciaRepository.Remove(entityNovo);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(ObraPendencia),
                Acao = "Atualizado",
                Descricao = $"Pendência '{entityNovo.Descricao}' atualizado",
                DadosAnteriores = JsonSerializer.Serialize(entityAntigo),
                DadosAtuais = JsonSerializer.Serialize(entityNovo)
            });

            await _unitOfWork.CommitAsync();

        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.ObraPendenciaRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraPendenciaRepository.Remove(entity);

            await _unitOfWork.CommitAsync();

        }
    }
}
