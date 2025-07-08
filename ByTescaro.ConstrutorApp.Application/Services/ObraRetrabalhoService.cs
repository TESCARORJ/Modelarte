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
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IUnitOfWork _unitOfWork;



        public ObraRetrabalhoService(IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogAuditoriaRepository logRepo, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logRepo = logRepo;
            _unitOfWork = unitOfWork;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";


        public async Task<List<ObraRetrabalhoDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraRetrabalhoRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraRetrabalhoDto>>(list);
        }

        public async Task CriarAsync(ObraRetrabalhoDto dto)
        {
            var entity = _mapper.Map<ObraRetrabalho>(dto); 
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            _unitOfWork.ObraRetrabalhoRepository.Add(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Criado",
                Descricao = $"Retrabalho '{entity.Descricao}' criado",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });

            await _unitOfWork.CommitAsync();

        }

        public async Task AtualizarAsync(ObraRetrabalhoDto dto)
        {
            var entityAntigo = await _unitOfWork.ObraRetrabalhoRepository.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<ObraRetrabalho>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            _unitOfWork.ObraRetrabalhoRepository.Update(entityNovo);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(ObraRetrabalho),
                Acao = "Atualizado",
                Descricao = $"Retrabalho '{entityNovo.Descricao}' atualizado",
                DadosAnteriores = JsonSerializer.Serialize(entityAntigo),
                DadosAtuais = JsonSerializer.Serialize(entityNovo)
            });

            await _unitOfWork.CommitAsync();

        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.ObraRetrabalhoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraRetrabalhoRepository.Remove(entity);
            await _unitOfWork.CommitAsync();
        }
    }
}
