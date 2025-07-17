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
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Isso garante que essa instância de 'equipamentoAntigoParaAuditoria' não será modificada.
            var equipamentoAntigoParaAuditoria = await _unitOfWork.EquipamentoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (equipamentoAntigoParaAuditoria == null)
            {
                // Se não encontrou o equipamento antigo, não há o que auditar nem atualizar.
                // Considere lançar uma exceção ou retornar um resultado de falha.
                throw new KeyNotFoundException($"Equipamento com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa é a instância que o EF Core vai monitorar para detectar as mudanças.
            var equipamentoParaAtualizar = await _unitOfWork.EquipamentoRepository.GetByIdAsync(dto.Id);

            if (equipamentoParaAtualizar == null)
            {
                // Isso deve ser raro se o `equipamentoAntigoParaAuditoria` foi encontrado,
                // mas é bom ter uma verificação.
                throw new KeyNotFoundException($"Equipamento com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'equipamentoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, equipamentoParaAtualizar);

            // Garanta que campos de auditoria de cadastro não sejam sobrescritos pelo DTO.
            // (Já estavam sendo feitos na sua versão original, apenas mantendo aqui para clareza).
            equipamentoParaAtualizar.UsuarioCadastroId = equipamentoAntigoParaAuditoria.UsuarioCadastroId;
            equipamentoParaAtualizar.DataHoraCadastro = equipamentoAntigoParaAuditoria.DataHoraCadastro;

            // O método .Update() no repositório muitas vezes não é estritamente necessário se
            // a entidade já está rastreada e suas propriedades foram alteradas.
            // O EF Core já detecta as mudanças automaticamente.
            // _unitOfWork.EquipamentoRepository.Update(equipamentoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'equipamentoAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'equipamentoParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(equipamentoAntigoParaAuditoria, equipamentoParaAtualizar, usuarioLogadoId);

            // 5. Salve as alterações no banco de dados.
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