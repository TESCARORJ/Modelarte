using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraServicoListaService : IObraServicoListaService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraServicoListaService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraServicoListaDto>> ObterPorObraIdAsync(long obraId)
        {
            var listas = await _unitOfWork.ObraServicoListaRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraServicoListaDto>>(listas);
        }

        public async Task<ObraServicoListaDto?> ObterPorIdAsync(long id)
        {
            var entity = await _unitOfWork.ObraServicoListaRepository.GetByIdWithItensAsync(id);
            return entity == null ? null : _mapper.Map<ObraServicoListaDto>(entity);
        }

        public async Task<ObraServicoListaDto> CriarAsync(ObraServicoListaDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<ObraServicoLista>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            // Se o DTO já vem com itens, o AutoMapper deve associá-los
            // Garantindo que a FK para a lista principal seja nula para que o EF possa preenchê-la.
            foreach (var item in entity.Itens)
            {
                item.ObraServicoListaId = 0;
                item.ObraId = dto.ObraId;
                item.UsuarioCadastroId = usuarioLogadoId;
                item.DataHoraCadastro = DateTime.Now;
            }

            _unitOfWork.ObraServicoListaRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();

            // O 'entity.Id' agora tem o valor gerado pelo banco.
            // Buscamos a entidade recém-criada com todos os seus relacionamentos.
            var entidadeSalva = await _unitOfWork.ObraServicoListaRepository.GetByIdWithItensAsync(entity.Id);

            // Agora mapeamos um único objeto para um único DTO, o que funciona.
            return _mapper.Map<ObraServicoListaDto>(entidadeSalva);
        }

        public async Task AtualizarAsync(ObraServicoListaDto dto)
        {
            // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga com os itens, SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'listaAntigaParaAuditoria' NÃO será modificada e representa o estado original.
            var listaAntigaParaAuditoria = await _unitOfWork.ObraServicoListaRepository
                .GetByIdWithItensNoTrackingAsync(dto.Id); // NOVO MÉTODO NECESSÁRIO

            if (listaAntigaParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Lista de Serviços da Obra com ID {dto.Id} não encontrada para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO e com os itens.
            // Essa instância 'listaParaAtualizar' é a que o EF Core está monitorando e será modificada.
            var listaParaAtualizar = await _unitOfWork.ObraServicoListaRepository
                .GetByIdWithItensAsync(dto.Id);

            if (listaParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Lista de Serviços da Obra com ID {dto.Id} não encontrada para atualização.");
            }

            // 3. Atualiza os dados principais da lista
            listaParaAtualizar.Data = dto.Data.ToDateTime(TimeOnly.MinValue);
            listaParaAtualizar.ResponsavelId = dto.ResponsavelId;

            // 4. Lógica para ATUALIZAR/ADICIONAR/REMOVER itens da coleção 'Itens'.
            // Em vez de Clear() e Add() de todos, que pode ser ineficiente e perde o rastreamento do EF,
            // vamos comparar e modificar apenas o necessário.

            // Itens a serem removidos (existem na lista antiga, mas não no DTO)
            var itensParaRemover = listaParaAtualizar.Itens
                .Where(existingItem => !dto.Itens.Any(dtoItem => dtoItem.Id == existingItem.Id && dtoItem.Id != 0))
                .ToList();

            foreach (var item in itensParaRemover)
            {
                listaParaAtualizar.Itens.Remove(item);
                // Se você tiver um repositório específico para ObraServico, pode ser bom marcar para remoção explícita:
                // _unitOfWork.ObraServicoRepository.Remove(item);
            }

            // Itens a serem adicionados ou atualizados
            foreach (var itemDto in dto.Itens)
            {
                var existingItem = listaParaAtualizar.Itens.FirstOrDefault(i => i.Id == itemDto.Id && i.Id != 0);

                if (existingItem == null) // Item novo (não tem ID ou não foi encontrado na lista existente)
                {
                    listaParaAtualizar.Itens.Add(new ObraServico
                    {
                        ServicoId = itemDto.ServicoId,
                        Quantidade = itemDto.Quantidade,
                        DataHoraCadastro = DateTime.Now,
                        UsuarioCadastroId = usuarioLogadoId,
                        // Certifique-se de que a FK para ObraServicoLista seja preenchida se não for feita por convenção do EF
                        ObraServicoListaId = listaParaAtualizar.Id
                    });
                }
                else // Item existente, atualizar
                {
                    // Atualize as propriedades que podem mudar
                    existingItem.ServicoId = itemDto.ServicoId;
                    existingItem.Quantidade = itemDto.Quantidade;
            

                    // Não é necessário chamar _unitOfWork.ObraServicoRepository.Update(existingItem);
                    // O EF Core já está rastreando existingItem e detectará as mudanças.
                }
            }

            // O _unitOfWork.ObraServicoListaRepository.Update(entity); é geralmente redundante aqui,
            // pois a entidade principal já está rastreada e suas propriedades e a coleção Itens foram modificadas.
            // O EF Core detectará as mudanças automaticamente.
            // _unitOfWork.ObraServicoListaRepository.Update(listaParaAtualizar);

            // 5. Registre a auditoria. Use RegistrarAtualizacaoAsync.
            // Certifique-se de que o seu AuditoriaService saiba como lidar com coleções aninhadas
            // ao comparar 'antigo' e 'novo'.
            await _auditoriaService.RegistrarAtualizacaoAsync(listaAntigaParaAuditoria, listaParaAtualizar, usuarioLogadoId);

            // 6. Salva TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraServicoListaRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraServicoListaRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }
    }
}
