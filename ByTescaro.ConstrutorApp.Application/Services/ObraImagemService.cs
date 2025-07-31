using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraImagemService : IObraImagemService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraImagemService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraImagemDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraImagemRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraImagemDto>>(list);
        }

        public async Task CriarAsync(ObraImagemDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            var entity = _mapper.Map<ObraImagem>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ObraImagemRepository.Add(entity);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);

            // Atualiza o DTO com o Id gerado pelo banco de dados
            dto.Id = entity.Id;
            dto.UsuarioCadastroNome = usuarioLogado?.Nome ?? "Sistema";
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            var entity = await _unitOfWork.ObraImagemRepository.GetByIdAsync(id);
            if (entity != null)
            {
                // Remover o arquivo físico
                try
                {
                    var caminhoFisico = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", entity.CaminhoRelativo.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (File.Exists(caminhoFisico))
                    {
                        File.Delete(caminhoFisico);
                    }
                }
                catch (Exception ex)
                {
                    // Logar o erro, mas não impedir a remoção do registro no BD
                    Console.WriteLine($"Erro ao excluir o arquivo físico da imagem: {ex.Message}");
                }

                _unitOfWork.ObraImagemRepository.Remove(entity);
                await _unitOfWork.CommitAsync();
                await _auditoriaService.RegistrarExclusaoAsync(_mapper.Map<ObraImagemDto>(entity), usuarioLogadoId);
            }
            else
            {
                throw new KeyNotFoundException($"Imagem com ID {id} não encontrada.");
            }
        }
    }
}
