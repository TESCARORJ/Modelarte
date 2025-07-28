using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Application.Mappings;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        // =========================================================================
        // MAPEAMENTOS GERAIS
        // =========================================================================

        CreateMap<LogAuditoriaDto, LogAuditoria>().ReverseMap();

        // Mapeamentos de DTO para Endereco (para criar/atualizar a entidade Endereco separadamente)
        // O AutoMapper tentará mapear propriedades com o mesmo nome automaticamente.
        // O .ForMember(dest => dest.Id, opt => opt.Ignore()) é crucial para que o Id não seja copiado
        // de um DTO para uma nova entidade Endereco, pois o DB gera o Id.
        CreateMap<ClienteDto, Endereco>()
           .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<FuncionarioDto, Endereco>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<FornecedorDto, Endereco>()
             .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UsuarioDto, Endereco>()
             .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<ProjetoDto, Endereco>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());


        // ==== Administração ====
        CreateMap<PerfilUsuario, PerfilUsuarioDto>().ReverseMap();

        CreateMap<Usuario, UsuarioDto>()
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
             .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
             .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Logradouro : string.Empty))
             .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Numero : string.Empty))
             .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Bairro : string.Empty))
             .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Cidade : string.Empty))
             .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Estado : string.Empty))
             .ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.UF : string.Empty))
             .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.CEP : string.Empty))
             .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Complemento : string.Empty))
             .ForMember(dest => dest.PerfilUsuario, opt => opt.MapFrom(src => src.PerfilUsuario));

        CreateMap<UsuarioDto, Usuario>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
            .ForMember(dest => dest.PerfilUsuarioId, opt => opt.MapFrom(src => src.PerfilUsuarioId))
            .ForMember(dest => dest.Endereco, opt => opt.Ignore())
            .ForMember(dest => dest.EnderecoId, opt => opt.Ignore())
            .ForMember(dest => dest.PerfilUsuario, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());


        // ==== Cadastros Básicos ====
        CreateMap<Cliente, ClienteDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Logradouro : string.Empty))
            .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Numero : string.Empty))
            .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Bairro : string.Empty))
            .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Cidade : string.Empty))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Estado : string.Empty))
            .ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.UF : string.Empty))
            .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.CEP : string.Empty))
            .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Complemento : string.Empty));

        CreateMap<ClienteDto, Cliente>()
            .ForMember(dest => dest.Endereco, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());


        CreateMap<Funcionario, FuncionarioDto>()
           .ForMember(dest => dest.FuncaoNome, opt => opt.MapFrom(src => src.Funcao != null ? src.Funcao.Nome : string.Empty))
           .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
           .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Logradouro : string.Empty))
           .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Numero : string.Empty))
           .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Bairro : string.Empty))
           .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Cidade : string.Empty))
           .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Estado : string.Empty))
           .ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.UF : string.Empty))
           .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.CEP : string.Empty))
           .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Complemento : string.Empty));

        CreateMap<FuncionarioDto, Funcionario>()
            .ForMember(dest => dest.Funcao, opt => opt.Ignore())
            .ForMember(dest => dest.Endereco, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());




        CreateMap<Funcao, FuncaoDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
;
        CreateMap<FuncaoDto, Funcao>()
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());



        CreateMap<Equipamento, EquipamentoDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty));

        CreateMap<EquipamentoDto, Equipamento>()
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());


        CreateMap<Fornecedor, FornecedorDto>()
        .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
        .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Logradouro : string.Empty))
        .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Numero : string.Empty))
        .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Bairro : string.Empty))
        .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Cidade : string.Empty))
        .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Estado : string.Empty))
        .ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.UF : string.Empty))
        .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.CEP : string.Empty))
        .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Complemento : string.Empty));

        CreateMap<FornecedorDto, Fornecedor>()
           .ForMember(dest => dest.Endereco, opt => opt.Ignore())
           .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());



        CreateMap<Insumo, InsumoDto>()
        .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty));

        CreateMap<InsumoDto, Insumo>()
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());

        CreateMap<FornecedorInsumo, FornecedorInsumoDto>()
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor != null ? src.Fornecedor.Nome : string.Empty))
            .ForMember(dest => dest.InsumoNome, opt => opt.MapFrom(src => src.Insumo != null ? src.Insumo.Nome : string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.Fornecedor, opt => opt.Ignore())
            .ForMember(dest => dest.Insumo, opt => opt.Ignore());

        CreateMap<Servico, ServicoDto>()
        .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty));

        CreateMap<ServicoDto, Servico>()
          .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());


        CreateMap<FornecedorServico, FornecedorServicoDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor != null ? src.Fornecedor.Nome : string.Empty))
            .ForMember(dest => dest.ServicoNome, opt => opt.MapFrom(src => src.Servico != null ? src.Servico.Nome : string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.Fornecedor, opt => opt.Ignore())
            .ForMember(dest => dest.Servico, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());


        // ==== Projeto ====
        CreateMap<Projeto, ProjetoDto>()
             .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
             .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Logradouro : string.Empty))
             .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Numero : string.Empty))
             .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Bairro : string.Empty))
             .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Cidade : string.Empty))
             .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Estado : string.Empty))
             .ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.UF : string.Empty))
             .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.CEP : string.Empty))
             .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Complemento : string.Empty))
             .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DataInicio)))
             .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim.HasValue ? DateOnly.FromDateTime(src.DataFim.Value) : (DateOnly?)null))
             .ReverseMap()
             .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.DataInicio.HasValue ? src.DataInicio.Value.ToDateTime(TimeOnly.MinValue) : default))
             .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim.HasValue ? src.DataFim.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null))
             .ForMember(dest => dest.Obras, opt => opt.Ignore())
             .ForMember(dest => dest.Endereco, opt => opt.Ignore())
             //.ForMember(dest => dest.Cliente, opt => opt.Ignore())
             .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());


        CreateMap<Projeto, ProjetoListDto>()
           .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DataInicio)))
            .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim.HasValue ? DateOnly.FromDateTime(src.DataFim.Value) : (DateOnly?)null))
            .ForMember(dest => dest.Obras, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.DataInicio.HasValue ? src.DataInicio.Value.ToDateTime(TimeOnly.MinValue) : default))
            .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim.HasValue ? src.DataFim.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null))
            .ForMember(dest => dest.Obras, opt => opt.Ignore())
            .ForMember(dest => dest.Endereco, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore()); 

        // ==== Obra ====
        CreateMap<Obra, ObraDto>()
           .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
           .ForMember(dest => dest.ProjetoNome, opt => opt.MapFrom(src => src.Projeto.Nome))
           .ForMember(dest => dest.ClienteNome, opt => opt.MapFrom(src => src.Projeto.Cliente.Nome));

        CreateMap<ObraDto, Obra>()
           .ForMember(dest => dest.Id, opt => opt.Ignore())
           .ForMember(dest => dest.Projeto, opt => opt.Ignore())
           .ForMember(dest => dest.Etapas, opt => opt.Ignore())
           .ForMember(dest => dest.Funcionarios, opt => opt.MapFrom(src => src.Funcionarios))
           .ForMember(dest => dest.Insumos, opt => opt.Ignore())
           .ForMember(dest => dest.ListasInsumo, opt => opt.Ignore())
           .ForMember(dest => dest.Servicos, opt => opt.Ignore())
           .ForMember(dest => dest.ListasServico, opt => opt.Ignore())
           .ForMember(dest => dest.Equipamentos, opt => opt.MapFrom(src => src.Equipamentos))
           .ForMember(dest => dest.Retrabalhos, opt => opt.Ignore())
           .ForMember(dest => dest.Pendencias, opt => opt.Ignore())
           .ForMember(dest => dest.Documentos, opt => opt.Ignore())
           .ForMember(dest => dest.Imagens, opt => opt.Ignore())
           .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());


        // ==== ObraFuncionario ====
        CreateMap<ObraFuncionario, ObraFuncionarioDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.Funcionario, opt => opt.Ignore())
            .ForMember(dest => dest.Obra, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());


        // ==== ObraFornecedor ====
        CreateMap<ObraFornecedor, ObraFornecedorDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.Fornecedor, opt => opt.Ignore())
            .ForMember(dest => dest.Obra, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());

        // ==== ObraInsumo ====
        CreateMap<ObraInsumo, ObraInsumoDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.InsumoNome, opt => opt.MapFrom(src => src.Insumo != null ? src.Insumo.Nome : string.Empty)) // Checagem de nulo
            .ForMember(dest => dest.UnidadeMedida, opt => opt.MapFrom(src => src.Insumo != null ? src.Insumo.UnidadeMedida : 0)) // Checagem de nulo
            .ReverseMap()
            .ForMember(dest => dest.Insumo, opt => opt.Ignore())
            .ForMember(dest => dest.Lista, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());

        // ==== ObraInsumoLista ====
        CreateMap<ObraInsumoLista, ObraInsumoListaDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.NomeResponsavel, opt => opt.MapFrom(src => src.Responsavel != null ? src.Responsavel.Nome : string.Empty)) // Checagem de nulo
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Data)))
            .ReverseMap()
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data.ToDateTime(TimeOnly.MinValue)))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ForMember(dest => dest.Responsavel, opt => opt.Ignore())
            .ForMember(dest => dest.Obra, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());

        // ==== ObraServico ====
        CreateMap<ObraServico, ObraServicoDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.ServicoNome, opt => opt.MapFrom(src => src.Servico != null ? src.Servico.Nome : string.Empty)) // Checagem de nulo
            .ReverseMap()
            .ForMember(dest => dest.Servico, opt => opt.Ignore())
            .ForMember(dest => dest.Lista, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());

        // ==== ObraServicoLista ====
        CreateMap<ObraServicoLista, ObraServicoListaDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.NomeResponsavel, opt => opt.MapFrom(src => src.Responsavel != null ? src.Responsavel.Nome : string.Empty)) // Checagem de nulo
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Data)))
            .ReverseMap()
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data.ToDateTime(TimeOnly.MinValue)))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ForMember(dest => dest.Responsavel, opt => opt.Ignore())
            .ForMember(dest => dest.Obra, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());


        // ==== ObraEquipamento ====
        CreateMap<ObraEquipamento, ObraEquipamentoDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.Equipamento, opt => opt.Ignore())
            .ForMember(dest => dest.Obra, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());

        // ==== Etapas Padrão ====
        CreateMap<ObraEtapaPadrao, ObraEtapaPadraoDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ReverseMap()
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());



        // Mapeamento combinado para ObraItemEtapaPadrao
        CreateMap<ObraItemEtapaPadrao, ObraItemEtapaPadraoDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.ObraEtapaPadraoId, opt => opt.MapFrom(src => src.ObraEtapaPadrao != null ? src.ObraEtapaPadrao.Id : src.ObraEtapaPadraoId))
            .ForMember(dest => dest.ObraEtapaPadraoNome, opt => opt.MapFrom(src => src.ObraEtapaPadrao != null ? src.ObraEtapaPadrao.Nome : string.Empty))
            .ForMember(dest => dest.Insumos, opt => opt.MapFrom(src => src.Insumos)) 
            .ReverseMap()
            .ForMember(dest => dest.ObraEtapaPadrao, opt => opt.Ignore())
            .ForMember(dest => dest.Insumos, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());

        // ==== Etapas Executadas ====
        CreateMap<ObraEtapa, ObraEtapaDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());
        CreateMap<ObraItemEtapa, ObraItemEtapaDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // ==== ObraPendencia ====
        CreateMap<ObraPendencia, ObraPendenciaDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.NomeResponsavel, opt => opt.MapFrom(src => src.Responsavel != null ? src.Responsavel.Nome : string.Empty)) // Checagem de nulo
            .ReverseMap()
            .ForMember(dest => dest.Obra, opt => opt.Ignore())
            .ForMember(dest => dest.Responsavel, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());

        // ==== ObraRetrabalho ====
        CreateMap<ObraRetrabalho, ObraRetrabalhoDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.NomeResponsavel, opt => opt.MapFrom(src => src.Responsavel != null ? src.Responsavel.Nome : string.Empty)) // Checagem de nulo
            .ReverseMap()
            .ForMember(dest => dest.Obra, opt => opt.Ignore())
            .ForMember(dest => dest.Responsavel, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());

        CreateMap<ObraDocumentoDto, ObraDocumento>()
            .ForMember(d => d.Obra, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<ObraImagemDto, ObraImagem>()
            .ForMember(d => d.Obra, opt => opt.Ignore())
            .ReverseMap();


        // ==== ObraItemEtapaPadraoInsumo ====
        CreateMap<ObraItemEtapaPadraoInsumo, ObraItemEtapaPadraoInsumoDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.InsumoNome, opt => opt.MapFrom(src => src.Insumo != null ? src.Insumo.Nome : string.Empty)) // Checagem de nulo
            .ReverseMap()
            .ForMember(dest => dest.Insumo, opt => opt.Ignore())
            .ForMember(dest => dest.ObraItemEtapaPadrao, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());

        // ==== Orçamento ====
        CreateMap<Orcamento, OrcamentoDto>().ReverseMap();
        CreateMap<OrcamentoItem, OrcamentoItemDto>()
            .ForMember(dest => dest.UsuarioCadastroNome, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty))
            .ForMember(dest => dest.InsumoNome, opt => opt.MapFrom(src => src.Insumo != null ? src.Insumo.Nome : string.Empty)) // Checagem de nulo
            .ForMember(dest => dest.ServicoNome, opt => opt.MapFrom(src => src.Servico != null ? src.Servico.Nome : string.Empty)) // Checagem de nulo
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor != null ? src.Fornecedor.Nome : string.Empty)) // Checagem de nulo
            .ReverseMap()
            .ForMember(dest => dest.UsuarioCadastro, opt => opt.Ignore());


        // ==== Agenda ====

        CreateMap<Evento, EventoDto>()
                .ForMember(dest => dest.NomeCriador, opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.Nome : string.Empty)) // Checagem de nulo
                .ForMember(dest => dest.Participantes, opt => opt.MapFrom(src => src.Participantes));

        CreateMap<CriarEventoRequest, Evento>();
        CreateMap<AtualizarEventoRequest, Evento>();
        CreateMap<ParticipanteEvento, ParticipanteEventoDto>()
               .ForMember(dest => dest.NomeUsuario, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Nome : string.Empty)); // Checagem de nulo
        CreateMap<LembreteEvento, LembreteEventoDto>().ReverseMap(); // Adicionado ReverseMap para simetria
        CreateMap<ConfiguracaoLembreteDiario, ConfiguracaoLembreteDiarioDto>().ReverseMap(); // Adicionado ReverseMap para simetria
        CreateMap<CriarConfiguracaoLembreteDiarioRequest, ConfiguracaoLembreteDiario>();
        CreateMap<AtualizarConfiguracaoLembreteDiarioRequest, ConfiguracaoLembreteDiario>();
    }
}