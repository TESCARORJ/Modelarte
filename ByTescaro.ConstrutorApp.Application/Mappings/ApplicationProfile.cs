using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Mappings;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        // =========================================================================
        // MAPEAMENTO DE ENDEREÇO (DTO PARA ENTIDADE ENDERECO)
        // Isso é para criar/atualizar a entidade Endereco em si.
        // =========================================================================
        CreateMap<ClienteDto, Endereco>()
           .ForMember(dest => dest.Id, opt => opt.Ignore())
           .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Logradouro))
           .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Numero))
           .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Bairro))
           .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Cidade))
           .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
           .ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.UF))
           .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.CEP))
           .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Complemento));


        CreateMap<FuncionarioDto, Endereco>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Logradouro))
            .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Numero))
            .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Bairro))
            .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Cidade))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
            .ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.UF))
            .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.CEP))
            .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Complemento));


        CreateMap<FornecedorDto, Endereco>();
            //.ForMember(dest => dest.Id, opt => opt.Ignore())
            //.ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Logradouro))
            //.ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Numero))
            //.ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Bairro))
            //.ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Cidade))
            //.ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
            //.ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.UF))
            //.ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.CEP))
            //.ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Complemento));


        CreateMap<UsuarioDto, Endereco>();
            //.ForMember(dest => dest.Id, opt => opt.Ignore())
            //.ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Logradouro))
            //.ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Numero))
            //.ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Bairro))
            //.ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Cidade))
            //.ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
            //.ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.UF))
            //.ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.CEP))
            //.ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Complemento));

        CreateMap<ProjetoDto, Endereco>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Logradouro))
            .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Numero))
            .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Bairro))
            .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Cidade))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
            .ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.UF))
            .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.CEP))
            .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Complemento));


        // ==== Administração ==== 
        CreateMap<PerfilUsuario, PerfilUsuarioDto>().ReverseMap();

        CreateMap<Usuario, UsuarioDto>();
            // CORRIGIDO: Tratamento de nulo para Endereco
            //.ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Logradouro : string.Empty))
            //.ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Numero : string.Empty))
            //.ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Bairro : string.Empty))
            //.ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Cidade : string.Empty))
            //.ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Estado : string.Empty))
            //.ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.UF : string.Empty))
            //.ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.CEP : string.Empty))
            //.ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Complemento : string.Empty));

        CreateMap<UsuarioDto, Usuario>()
            .ForMember(dest => dest.Endereco, opt => opt.Ignore()); // Endereco será gerenciado no serviço.


        // ==== Cadastros Básicos ==== 
        CreateMap<Cliente, ClienteDto>()
            // CORRIGIDO: Tratamento de nulo para Endereco
            .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Logradouro : string.Empty))
            .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Numero : string.Empty))
            .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Bairro : string.Empty))
            .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Cidade : string.Empty))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Estado : string.Empty))
            .ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.UF : string.Empty))
            .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.CEP : string.Empty))
            .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Complemento : string.Empty));

        CreateMap<ClienteDto, Cliente>()
            .ForMember(dest => dest.Endereco, opt => opt.Ignore()); // Endereco será gerenciado no serviço.


        CreateMap<Funcionario, FuncionarioDto>()
           .ForMember(dest => dest.FuncaoNome, opt => opt.MapFrom(src => src.Funcao != null ? src.Funcao.Nome : string.Empty)) // CORRIGIDO: Tratamento de nulo para Funcao
                                                                                                                               // CORRIGIDO: Adicionado tratamento de nulo para Endereco
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
            .ForMember(dest => dest.Endereco, opt => opt.Ignore()); // Endereco será gerenciado no serviço.


        CreateMap<Funcao, FuncaoDto>().ReverseMap();
        CreateMap<Equipamento, EquipamentoDto>().ReverseMap();

        CreateMap<Fornecedor, FornecedorDto>();
           // CORRIGIDO: Tratamento de nulo para Endereco
           //.ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Logradouro : string.Empty))
           //.ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Numero : string.Empty))
           //.ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Bairro : string.Empty))
           //.ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Cidade : string.Empty))
           //.ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Estado : string.Empty))
           //.ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.UF : string.Empty))
           //.ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.CEP : string.Empty))
           //.ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco != null ? src.Endereco.Complemento : string.Empty));

        CreateMap<FornecedorDto, Fornecedor>()
           .ForMember(dest => dest.Endereco, opt => opt.Ignore());


        CreateMap<Insumo, InsumoDto>().ReverseMap();
        CreateMap<FornecedorInsumo, FornecedorInsumoDto>()
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor.Nome))
            .ForMember(dest => dest.InsumoNome, opt => opt.MapFrom(src => src.Insumo.Nome))
            .ReverseMap()
            .ForMember(dest => dest.Fornecedor, opt => opt.Ignore())
            .ForMember(dest => dest.Insumo, opt => opt.Ignore());

        CreateMap<Servico, ServicoDto>().ReverseMap();
        CreateMap<FornecedorServico, FornecedorServicoDto>()
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor.Nome))
            .ForMember(dest => dest.ServicoNome, opt => opt.MapFrom(src => src.Servico.Nome))
            .ReverseMap()
            .ForMember(dest => dest.Fornecedor, opt => opt.Ignore())
            .ForMember(dest => dest.Servico, opt => opt.Ignore());


        // ==== Projeto ==== 
        CreateMap<Projeto, ProjetoDto>()
            .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DataInicio)))
            .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim.HasValue ? DateOnly.FromDateTime(src.DataFim.Value) : (DateOnly?)null))
            .ReverseMap()
            .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.DataInicio.HasValue ? src.DataInicio.Value.ToDateTime(TimeOnly.MinValue) : default))
            .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim.HasValue ? src.DataFim.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null))
            .ForMember(dest => dest.Obras, opt => opt.Ignore());

        CreateMap<Projeto, ProjetoListDto>()
            .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DataInicio)))
            .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim.HasValue ? DateOnly.FromDateTime(src.DataFim.Value) : (DateOnly?)null))
            .ForMember(dest => dest.Obras, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.DataInicio.HasValue ? src.DataInicio.Value.ToDateTime(TimeOnly.MinValue) : default))
            .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim.HasValue ? src.DataFim.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null))
            .ForMember(dest => dest.Obras, opt => opt.Ignore());

        // ==== Obra ==== 
        CreateMap<Obra, ObraDto>()
        .ForMember(dest => dest.Documentos, opt => opt.MapFrom(src => src.Documentos))
        .ForMember(dest => dest.Imagens, opt => opt.MapFrom(src => src.Imagens))
        .ReverseMap()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.Projeto, opt => opt.Ignore()) // evitar loop ou entidade já rastreada 
        .ForMember(dest => dest.Etapas, opt => opt.Ignore())
        .ForMember(dest => dest.Funcionarios, opt => opt.Ignore())
        .ForMember(dest => dest.Insumos, opt => opt.Ignore())
        .ForMember(dest => dest.ListasInsumo, opt => opt.Ignore())
        .ForMember(dest => dest.Servicos, opt => opt.Ignore())
        .ForMember(dest => dest.ListasServico, opt => opt.Ignore())
        .ForMember(dest => dest.Equipamentos, opt => opt.Ignore())
        .ForMember(dest => dest.Retrabalhos, opt => opt.Ignore())
        .ForMember(dest => dest.Pendencias, opt => opt.Ignore())
        .ForMember(dest => dest.Documentos, opt => opt.Ignore())
        .ForMember(dest => dest.Imagens, opt => opt.Ignore());


        // ==== ObraFuncionario ==== 
        CreateMap<ObraFuncionario, ObraFuncionarioDto>()
            .ReverseMap()
            .ForMember(dest => dest.Funcionario, opt => opt.Ignore())
            .ForMember(dest => dest.Obra, opt => opt.Ignore());

        // ==== ObraFornecedor ==== 
        CreateMap<ObraFornecedor, ObraFornecedorDto>()
            .ReverseMap()
            .ForMember(dest => dest.Fornecedor, opt => opt.Ignore())
            .ForMember(dest => dest.Obra, opt => opt.Ignore());

        // ==== ObraInsumo ==== 
        CreateMap<ObraInsumo, ObraInsumoDto>()
            .ForMember(dest => dest.InsumoNome, opt => opt.MapFrom(src => src.Insumo.Nome))
            .ForMember(dest => dest.UnidadeMedida, opt => opt.MapFrom(src => src.Insumo.UnidadeMedida))
            .ReverseMap()
            .ForMember(dest => dest.Insumo, opt => opt.Ignore())
            .ForMember(dest => dest.Lista, opt => opt.Ignore());

        // ==== ObraInsumoLista ==== 
        CreateMap<ObraInsumoLista, ObraInsumoListaDto>()
            .ForMember(dest => dest.NomeResponsavel, opt => opt.MapFrom(src => src.Responsavel.Nome))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Data)))
            .ReverseMap()
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data.ToDateTime(TimeOnly.MinValue)))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ForMember(dest => dest.Responsavel, opt => opt.Ignore())
            .ForMember(dest => dest.Obra, opt => opt.Ignore());

        // ==== ObraServico ==== 
        CreateMap<ObraServico, ObraServicoDto>()
            .ForMember(dest => dest.ServicoNome, opt => opt.MapFrom(src => src.Servico.Nome))
            .ReverseMap()
            .ForMember(dest => dest.Servico, opt => opt.Ignore())
            .ForMember(dest => dest.Lista, opt => opt.Ignore());

        // ==== ObraServicoLista ==== 
        CreateMap<ObraServicoLista, ObraServicoListaDto>()
            .ForMember(dest => dest.NomeResponsavel, opt => opt.MapFrom(src => src.Responsavel.Nome))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Data)))
            .ReverseMap()
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data.ToDateTime(TimeOnly.MinValue)))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ForMember(dest => dest.Responsavel, opt => opt.Ignore())
            .ForMember(dest => dest.Obra, opt => opt.Ignore());


        // ==== ObraEquipamento ==== 
        CreateMap<ObraEquipamento, ObraEquipamentoDto>()
            .ReverseMap()
            .ForMember(dest => dest.Equipamento, opt => opt.Ignore())
            .ForMember(dest => dest.Obra, opt => opt.Ignore());

        // ==== Etapas Padrão ==== 

        CreateMap<ObraItemEtapaPadrao, ObraItemEtapaPadraoDto>()
            .ForMember(dest => dest.ObraEtapaPadraoId, opt => opt.MapFrom(src => src.ObraEtapaPadrao != null ? src.ObraEtapaPadrao.Id : src.ObraEtapaPadraoId))
            .ForMember(dest => dest.ObraEtapaPadraoNome, opt => opt.MapFrom(src => src.ObraEtapaPadrao != null ? src.ObraEtapaPadrao.Nome : ""))
            .ReverseMap()
            .ForMember(dest => dest.ObraEtapaPadrao, opt => opt.Ignore());

        CreateMap<ObraEtapaPadrao, ObraEtapaPadraoDto>()
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ReverseMap();

        // ==== Etapas Executadas ==== 
        CreateMap<ObraEtapa, ObraEtapaDto>()
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<ObraItemEtapa, ObraItemEtapaDto>()
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // ==== ObraPendencia ==== 
        CreateMap<ObraPendencia, ObraPendenciaDto>()
            .ForMember(dest => dest.NomeResponsavel, opt => opt.MapFrom(src => src.Responsavel.Nome))
            .ReverseMap()
            .ForMember(dest => dest.Obra, opt => opt.Ignore())
            .ForMember(dest => dest.Responsavel, opt => opt.Ignore());

        // ==== ObraRetrabalho ==== 
        CreateMap<ObraRetrabalho, ObraRetrabalhoDto>()
            .ForMember(dest => dest.NomeResponsavel, opt => opt.MapFrom(src => src.Responsavel.Nome))
            .ReverseMap()
            .ForMember(dest => dest.Obra, opt => opt.Ignore())
            .ForMember(dest => dest.Responsavel, opt => opt.Ignore());

        CreateMap<ObraDocumentoDto, ObraDocumento>()
            .ForMember(d => d.Obra, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<ObraImagemDto, ObraImagem>()
            .ForMember(d => d.Obra, opt => opt.Ignore())
            .ReverseMap();


        // ==== ObraItemEtapaPadraoInsumo ==== 
        CreateMap<ObraItemEtapaPadraoInsumo, ObraItemEtapaPadraoInsumoDto>()
            .ForMember(dest => dest.InsumoNome, opt => opt.MapFrom(src => src.Insumo.Nome))
            .ReverseMap()
            .ForMember(dest => dest.Insumo, opt => opt.Ignore())
            .ForMember(dest => dest.ObraItemEtapaPadrao, opt => opt.Ignore());


        CreateMap<ObraItemEtapaPadrao, ObraItemEtapaPadraoDto>()
            .ForMember(dest => dest.ObraEtapaPadraoId, opt => opt.MapFrom(src => src.ObraEtapaPadrao != null ? src.ObraEtapaPadrao.Id : src.ObraEtapaPadraoId))
            .ForMember(dest => dest.ObraEtapaPadraoNome, opt => opt.MapFrom(src => src.ObraEtapaPadrao != null ? src.ObraEtapaPadrao.Nome : ""))
            .ForMember(dest => dest.Insumos, opt => opt.MapFrom(src => src.Insumos))
            .ReverseMap()
            .ForMember(dest => dest.ObraEtapaPadrao, opt => opt.Ignore())
            .ForMember(dest => dest.Insumos, opt => opt.Ignore());
        // ==== Orçamento ==== 
        CreateMap<Orcamento, OrcamentoDto>().ReverseMap();
        CreateMap<OrcamentoItem, OrcamentoItemDto>()
            .ForMember(dest => dest.InsumoNome, opt => opt.MapFrom(src => src.Insumo.Nome))
            .ForMember(dest => dest.ServicoNome, opt => opt.MapFrom(src => src.Servico.Nome))
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor.Nome))
            .ReverseMap();



    }
}