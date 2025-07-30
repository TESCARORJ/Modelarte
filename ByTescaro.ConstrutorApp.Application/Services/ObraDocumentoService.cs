using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraDocumentoService : IObraDocumentoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraDocumentoService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraDocumentoDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraDocumentoRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraDocumentoDto>>(list);
        }

        public async Task CriarAsync(ObraDocumentoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            var entity = _mapper.Map<ObraDocumento>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ObraDocumentoRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
            await _unitOfWork.CommitAsync();

            // Atualiza o DTO com o Id gerado pelo banco de dados
            dto.Id = entity.Id;
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            var entity = await _unitOfWork.ObraDocumentoRepository.GetByIdAsync(id);
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
                    Console.WriteLine($"Erro ao excluir o arquivo físico: {ex.Message}");
                }

                _unitOfWork.ObraDocumentoRepository.Remove(entity);
                await _auditoriaService.RegistrarExclusaoAsync(_mapper.Map<ObraDocumentoDto>(entity), usuarioLogadoId);
                await _unitOfWork.CommitAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Documento com ID {id} não encontrado.");
            }
        }
    }
}
