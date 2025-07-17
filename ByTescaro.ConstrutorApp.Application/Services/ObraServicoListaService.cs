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
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraServicoListaRepository.GetByIdWithItensAsync(dto.Id);
            if (entity == null) return;

            // Atualiza os dados principais da lista
            entity.Data = dto.Data.ToDateTime(TimeOnly.MinValue);
            entity.ResponsavelId = dto.ResponsavelId;

            // Atualiza os itens (remoção e adição simples)
            entity.Itens.Clear();
            foreach (var itemDto in dto.Itens)
            {

                entity.Itens.Add(new ObraServico
                {
                    ServicoId = itemDto.ServicoId,
                    Quantidade = itemDto.Quantidade,
                    DataHoraCadastro = DateTime.Now,
                    UsuarioCadastroId = usuarioLogadoId
                });
            }

            _unitOfWork.ObraServicoListaRepository.Update(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
            // Não é necessário registrar uma atualização, pois estamos tratando como uma nova criação
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
