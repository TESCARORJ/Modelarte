using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class EquipamentoService : IEquipamentoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        private readonly IAuditoriaService _auditoriaService;

        public EquipamentoService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUsuarioLogadoService usuarioLogadoService,
            IAuditoriaService auditoriaService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _usuarioLogadoService = usuarioLogadoService;
            _auditoriaService = auditoriaService;
        }



        public async Task<IEnumerable<EquipamentoDto>> ObterTodosAsync()
        {
            var equipamentos = await _unitOfWork.EquipamentoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EquipamentoDto>>(equipamentos);
        }

        public async Task<EquipamentoDto?> ObterPorIdAsync(long id)
        {
            var equipamento = await _unitOfWork.EquipamentoRepository.GetByIdAsync(id);
            return equipamento == null ? null : _mapper.Map<EquipamentoDto>(equipamento);
        }

        public async Task CriarAsync(EquipamentoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;


            var entity = _mapper.Map<Equipamento>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            entity.Status = Domain.Enums.StatusEquipamento.Disponivel;

            _unitOfWork.EquipamentoRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);



            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(EquipamentoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;


            var entityAntigo = await _unitOfWork.EquipamentoRepository.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<Equipamento>(dto);
            entityNovo.UsuarioCadastroId = entityAntigo.UsuarioCadastroId;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            _unitOfWork.EquipamentoRepository.Update(entityNovo);
            await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, usuarioLogadoId);


            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;


            var entity = await _unitOfWork.EquipamentoRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.EquipamentoRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);


            await _unitOfWork.CommitAsync();
        }

        //public async Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync()
        //{
        //    return await _repo.ObterResumoAlocacaoAsync();
        //}

        public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(nome)) return false;
            return await _unitOfWork.EquipamentoRepository.ExistsAsync(e =>
                e.Nome == nome && (ignoreId == null || e.Id != ignoreId.Value));
        }

        public async Task<bool> PatrimonioExistsAsync(string patrimonio, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(patrimonio)) return false;
            return await _unitOfWork.EquipamentoRepository.ExistsAsync(e =>
                e.Patrimonio == patrimonio && (ignoreId == null || e.Id != ignoreId.Value));
        }
    }

}