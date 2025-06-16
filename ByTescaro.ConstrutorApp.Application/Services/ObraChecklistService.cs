using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraChecklistService : IObraChecklistService
    {
        private readonly IObraEtapaRepository _etapaRepo;
        private readonly IObraItemEtapaRepository _itemRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ObraChecklistService(IObraEtapaRepository etapaRepo, IObraItemEtapaRepository itemRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _etapaRepo = etapaRepo;
            _itemRepo = itemRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private string UsuarioLogado =>
    _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<List<ObraEtapaDto>> ObterChecklistAsync(long obraId)
        {
            var etapas = await _etapaRepo.GetByObraIdAsync(obraId);
            foreach (var etapa in etapas)
                etapa.Itens = await _itemRepo.GetByEtapaIdAsync(etapa.Id);

            return _mapper.Map<List<ObraEtapaDto>>(etapas);
        }

        public async Task SalvarChecklistAsync(long obraId, List<ObraEtapaDto> etapasDto)
        {
            var etapasAtuais = await _etapaRepo.GetByObraIdAsync(obraId);

            // Remoção
            var removidas = etapasAtuais.Where(e => !etapasDto.Any(dto => dto.Id == e.Id)).ToList();
            foreach (var etapa in removidas)
                await _etapaRepo.RemoveAsync(etapa);

            // Adição/Atualização
            foreach (var etapaDto in etapasDto)
            {
                if (etapaDto.Id == 0)
                {
                    var novaEtapa = _mapper.Map<ObraEtapa>(etapaDto);
                    novaEtapa.ObraId = obraId;
                    novaEtapa.Itens = new List<ObraItemEtapa>(); // evita duplicação automática

                    await _etapaRepo.AddAsync(novaEtapa);

                    foreach (var itemDto in etapaDto.Itens)
                    {
                        var novoItem = _mapper.Map<ObraItemEtapa>(itemDto);
                        novoItem.ObraEtapaId = novaEtapa.Id;
                        await _itemRepo.AddAsync(novoItem);
                    }
                }

                else
                {
                    var etapaExistente = etapasAtuais.FirstOrDefault(e => e.Id == etapaDto.Id);
                    if (etapaExistente is null) continue;

                    etapaExistente.Nome = etapaDto.Nome;
                    etapaExistente.Ordem = etapaDto.Ordem;
                    etapaExistente.Status = etapaDto.Status;
                    etapaExistente.DataInicio = etapaDto.DataInicio;
                    etapaExistente.DataConclusao = etapaDto.DataConclusao;

                    await _etapaRepo.UpdateAsync(etapaExistente);

                    var itensAtuais = await _itemRepo.GetByEtapaIdAsync(etapaExistente.Id);

                    var itensRemovidos = itensAtuais
                        .Where(i => !etapaDto.Itens.Any(dto => dto.Id == i.Id)).ToList();

                    foreach (var item in itensRemovidos)
                        await _itemRepo.RemoveAsync(item);

                    foreach (var itemDto in etapaDto.Itens)
                    {
                        if (itemDto.Id == 0)
                        {
                            var novo = _mapper.Map<ObraItemEtapa>(itemDto);
                            novo.ObraEtapaId = etapaExistente.Id;
                            await _itemRepo.AddAsync(novo);
                        }
                        else
                        {
                            var existente = itensAtuais.FirstOrDefault(i => i.Id == itemDto.Id);
                            if (existente != null)
                            {
                                existente.Nome = itemDto.Nome;
                                existente.Ordem = itemDto.Ordem;
                                existente.Concluido = itemDto.Concluido;
                                existente.IsDataPrazo = itemDto.IsDataPrazo;
                                existente.DiasPrazo = itemDto.DiasPrazo;
                                existente.PrazoAtivo = itemDto.PrazoAtivo;
                                existente.DataConclusao = itemDto.DataConclusao;
                                await _itemRepo.UpdateAsync(existente);
                            }
                        }
                    }
                }
            }
        }
    }


}
