using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraItemEtapaPadraoService : IObraItemEtapaPadraoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraItemEtapaPadraoService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }


        public async Task<ObraItemEtapaPadraoDto?> ObterPorIdAsync(long id)
        {
            var entity = await _unitOfWork.ObraItemEtapaPadraoRepository.GetByIdAsync(id);
            return _mapper.Map<ObraItemEtapaPadraoDto>(entity);
        }
        public async Task<List<ObraItemEtapaPadraoDto>> ObterTodasAsync()
        {
            var list = await _unitOfWork.ObraItemEtapaPadraoRepository.FindAllWithIncludesAsync(x => x.Id > 0, x => x.ObraEtapaPadrao);
            return _mapper.Map<List<ObraItemEtapaPadraoDto>>(list);
        }
        public async Task<List<ObraItemEtapaPadraoDto>> ObterPorEtapaIdAsync(long etapaId)
        {
            var itens = await _unitOfWork.ObraItemEtapaPadraoRepository.GetByEtapaPadraoIdAsync(etapaId);
            return _mapper.Map<List<ObraItemEtapaPadraoDto>>(itens);
        }
        public async Task AtualizarConclusaoAsync(long itemId, bool concluido)
        {
            var item = await _unitOfWork.ObraItemEtapaPadraoRepository.GetByIdAsync(itemId);
            if (item == null) return;

            // Como este é um item padrão, normalmente ele não teria um campo "Concluido"
            // Este método deve ser ignorado ou removido da interface
            // Caso queira manter, você deve adicionar esse campo na entidade
        }
        public async Task<ObraItemEtapaPadraoDto> CriarAsync(ObraItemEtapaPadraoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
           
            dto.Nome = dto.Nome.Trim();


            // VERIFICAÇÃO DE DUPLICIDADE
            if (await _unitOfWork.ObraItemEtapaPadraoRepository.JaExisteAsync(dto.Nome, dto.ObraEtapaPadraoId))
            {
                throw new DuplicateRecordException($"O item '{dto.Nome}' já existe para esta etapa padrão.");

            }

            var entity = _mapper.Map<ObraItemEtapaPadrao>(dto);


            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ObraItemEtapaPadraoRepository.Add(entity); // O 'entity' agora terá o ID gerado após o SaveChanges interno do AddAsync
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();

            // Mapeia a entidade atualizada de volta para um DTO e o retorna
            var createdDto = _mapper.Map<ObraItemEtapaPadraoDto>(entity);
            return createdDto;
        }
        public async Task AtualizarAsync(ObraItemEtapaPadraoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            dto.Nome = dto.Nome.Trim();

            var entityAntigo = await _unitOfWork.ObraItemEtapaPadraoRepository.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            // VERIFICAÇÃO DE DUPLICIDADE (ignorando o próprio ID)
            if (await _unitOfWork.ObraItemEtapaPadraoRepository.JaExisteAsync(dto.Nome, dto.ObraEtapaPadraoId, dto.Id))
            {
                throw new DuplicateRecordException($"O item '{dto.Nome}' já existe para esta etapa padrão.");
            }

           var entityNovo = _mapper.Map(dto, entityAntigo);
            _unitOfWork.ObraItemEtapaPadraoRepository.Update(entityNovo);
            await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }
        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.ObraItemEtapaPadraoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraItemEtapaPadraoRepository.Remove(entity);
            await _unitOfWork.CommitAsync();
        }

    }
}
