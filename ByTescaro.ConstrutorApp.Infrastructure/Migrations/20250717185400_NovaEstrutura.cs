using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NovaEstrutura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Endereco",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Logradouro = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Numero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UF = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    CEP = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    Complemento = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endereco", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipamento",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Patrimonio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CustoLocacaoDiaria = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipamento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Funcao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Insumo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UnidadeMedida = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogAuditoria",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<long>(type: "bigint", nullable: false),
                    UsuarioNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Entidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TipoLogAuditoria = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DadosAnteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DadosAtuais = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdEntidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogAuditoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObraEtapaPadrao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraEtapaPadrao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerfilUsuario",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilUsuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servico",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servico", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pessoa",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Sobrenome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CpfCnpj = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TipoPessoa = table.Column<int>(type: "int", nullable: false),
                    TipoEntidade = table.Column<int>(type: "int", nullable: false),
                    TelefonePrincipal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TelefoneWhatsApp = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EnderecoId = table.Column<long>(type: "bigint", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pessoa_Endereco_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Endereco",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ObraItemEtapaPadrao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraEtapaPadraoId = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    IsDataPrazo = table.Column<bool>(type: "bit", nullable: false),
                    DiasPrazo = table.Column<int>(type: "int", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraItemEtapaPadrao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraItemEtapaPadrao_ObraEtapaPadrao_ObraEtapaPadraoId",
                        column: x => x.ObraEtapaPadraoId,
                        principalTable: "ObraEtapaPadrao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cliente_Pessoa_Id",
                        column: x => x.Id,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fornecedor_Pessoa_Id",
                        column: x => x.Id,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Funcionario",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Salario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataAdmissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataDemissao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FuncaoId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Funcionario_Funcao_FuncaoId",
                        column: x => x.FuncaoId,
                        principalTable: "Funcao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Funcionario_Pessoa_Id",
                        column: x => x.Id,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PerfilUsuarioId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_PerfilUsuario_PerfilUsuarioId",
                        column: x => x.PerfilUsuarioId,
                        principalTable: "PerfilUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuario_Pessoa_Id",
                        column: x => x.Id,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObraItemEtapaPadraoInsumo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraItemEtapaPadraoId = table.Column<long>(type: "bigint", nullable: false),
                    InsumoId = table.Column<long>(type: "bigint", nullable: false),
                    QuantidadeSugerida = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraItemEtapaPadraoInsumo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraItemEtapaPadraoInsumo_Insumo_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObraItemEtapaPadraoInsumo_ObraItemEtapaPadrao_ObraItemEtapaPadraoId",
                        column: x => x.ObraItemEtapaPadraoId,
                        principalTable: "ObraItemEtapaPadrao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projeto",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TelefonePrincipal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EnderecoId = table.Column<long>(type: "bigint", nullable: true),
                    ClienteId = table.Column<long>(type: "bigint", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projeto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projeto_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projeto_Endereco_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Endereco",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FornecedorInsumo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FornecedorId = table.Column<long>(type: "bigint", nullable: false),
                    InsumoId = table.Column<long>(type: "bigint", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrazoEntregaDias = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FornecedorInsumo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FornecedorInsumo_Fornecedor_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FornecedorInsumo_Insumo_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FornecedorServico",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FornecedorId = table.Column<long>(type: "bigint", nullable: false),
                    ServicoId = table.Column<long>(type: "bigint", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrazoEntregaDias = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FornecedorServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FornecedorServico_Fornecedor_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FornecedorServico_Servico_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracaoLembreteDiario",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoraDoDia = table.Column<TimeSpan>(type: "time", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracaoLembreteDiario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracaoLembreteDiario_Usuario_UsuarioCadastroId",
                        column: x => x.UsuarioCadastroId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evento",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DataHoraInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataHoraFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRecorrente = table.Column<bool>(type: "bit", nullable: false),
                    FrequenciaRecorrencia = table.Column<int>(type: "int", nullable: true),
                    DataFimRecorrencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false),
                    Visibilidade = table.Column<int>(type: "int", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evento_Usuario_UsuarioCadastroId",
                        column: x => x.UsuarioCadastroId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Obra",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjetoId = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataInicioExecucao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponsavelMaterial = table.Column<int>(type: "int", nullable: false),
                    ResponsavelObraId = table.Column<long>(type: "bigint", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obra_Funcionario_ResponsavelObraId",
                        column: x => x.ResponsavelObraId,
                        principalTable: "Funcionario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Obra_Projeto_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projeto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                  
                });

            migrationBuilder.CreateTable(
                name: "LembreteEvento",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventoId = table.Column<long>(type: "bigint", nullable: false),
                    TempoAntes = table.Column<int>(type: "int", nullable: false),
                    UnidadeTempo = table.Column<int>(type: "int", nullable: true),
                    Enviado = table.Column<bool>(type: "bit", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LembreteEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LembreteEvento_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantesEvento",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventoId = table.Column<long>(type: "bigint", nullable: false),
                    UsuarioId = table.Column<long>(type: "bigint", nullable: false),
                    StatusParticipacao = table.Column<int>(type: "int", nullable: true),
                    DataResposta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantesEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantesEvento_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantesEvento_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObraDocumento",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    NomeOriginal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CaminhoRelativo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extensao = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TamanhoEmKb = table.Column<long>(type: "bigint", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraDocumento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraDocumento_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObraEquipamento",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    EquipamentoId = table.Column<long>(type: "bigint", nullable: false),
                    EquipamentoNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataInicioUso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFimUso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EquipamentoId1 = table.Column<long>(type: "bigint", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraEquipamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraEquipamento_Equipamento_EquipamentoId",
                        column: x => x.EquipamentoId,
                        principalTable: "Equipamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObraEquipamento_Equipamento_EquipamentoId1",
                        column: x => x.EquipamentoId1,
                        principalTable: "Equipamento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ObraEquipamento_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObraEtapa",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraEtapa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraEtapa_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObraFornecedor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    FornecedorId = table.Column<long>(type: "bigint", nullable: false),
                    FornecedorNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraFornecedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraFornecedor_Fornecedor_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObraFornecedor_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObraFuncionario",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    FuncionarioId = table.Column<long>(type: "bigint", nullable: false),
                    FuncionarioNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FuncaoNoObra = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FuncionarioId1 = table.Column<long>(type: "bigint", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraFuncionario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraFuncionario_Funcionario_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "Funcionario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObraFuncionario_Funcionario_FuncionarioId1",
                        column: x => x.FuncionarioId1,
                        principalTable: "Funcionario",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ObraFuncionario_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObraImagem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    NomeOriginal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CaminhoRelativo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TamanhoEmKb = table.Column<long>(type: "bigint", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraImagem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraImagem_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObraInsumoLista",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    ResponsavelId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraInsumoLista", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraInsumoLista_Funcionario_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Funcionario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObraInsumoLista_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObraPendencia",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResponsavelId = table.Column<long>(type: "bigint", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraPendencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraPendencia_Funcionario_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Funcionario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ObraPendencia_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObraRetrabalho",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResponsavelId = table.Column<long>(type: "bigint", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraRetrabalho", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraRetrabalho_Funcionario_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Funcionario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ObraRetrabalho_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObraServicoLista",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    ResponsavelId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraServicoLista", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraServicoLista_Funcionario_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Funcionario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObraServicoLista_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orcamento",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    Responsavel = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orcamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orcamento_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrcamentoObra",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Responsavel = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrcamentoObra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrcamentoObra_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObraItemEtapa",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraEtapaId = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Concluido = table.Column<bool>(type: "bit", nullable: false),
                    IsDataPrazo = table.Column<bool>(type: "bit", nullable: false),
                    DiasPrazo = table.Column<int>(type: "int", nullable: true),
                    PrazoAtivo = table.Column<bool>(type: "bit", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraItemEtapa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraItemEtapa_ObraEtapa_ObraEtapaId",
                        column: x => x.ObraEtapaId,
                        principalTable: "ObraEtapa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObraInsumo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    ObraInsumoListaId = table.Column<long>(type: "bigint", nullable: false),
                    InsumoId = table.Column<long>(type: "bigint", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraInsumo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraInsumo_Insumo_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObraInsumo_ObraInsumoLista_ObraInsumoListaId",
                        column: x => x.ObraInsumoListaId,
                        principalTable: "ObraInsumoLista",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObraInsumo_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObraServico",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    ObraServicoListaId = table.Column<long>(type: "bigint", nullable: false),
                    ServicoId = table.Column<long>(type: "bigint", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraServico_ObraServicoLista_ObraServicoListaId",
                        column: x => x.ObraServicoListaId,
                        principalTable: "ObraServicoLista",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObraServico_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObraServico_Servico_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrcamentoItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrcamentoObraId = table.Column<long>(type: "bigint", nullable: false),
                    InsumoId = table.Column<long>(type: "bigint", nullable: true),
                    ServicoId = table.Column<long>(type: "bigint", nullable: true),
                    FornecedorId = table.Column<long>(type: "bigint", nullable: true),
                    Quantidade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrcamentoId = table.Column<long>(type: "bigint", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrcamentoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrcamentoItem_Fornecedor_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrcamentoItem_Insumo_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrcamentoItem_OrcamentoObra_OrcamentoObraId",
                        column: x => x.OrcamentoObraId,
                        principalTable: "OrcamentoObra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrcamentoItem_Orcamento_OrcamentoId",
                        column: x => x.OrcamentoId,
                        principalTable: "Orcamento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrcamentoItem_Servico_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracaoLembreteDiario_UsuarioCadastroId",
                table: "ConfiguracaoLembreteDiario",
                column: "UsuarioCadastroId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipamento_Patrimonio",
                table: "Equipamento",
                column: "Patrimonio");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_UsuarioCadastroId",
                table: "Evento",
                column: "UsuarioCadastroId");

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorInsumo_FornecedorId",
                table: "FornecedorInsumo",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorInsumo_InsumoId",
                table: "FornecedorInsumo",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorServico_FornecedorId",
                table: "FornecedorServico",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorServico_ServicoId",
                table: "FornecedorServico",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionario_FuncaoId",
                table: "Funcionario",
                column: "FuncaoId");

            migrationBuilder.CreateIndex(
                name: "IX_LembreteEvento_EventoId",
                table: "LembreteEvento",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_LogAuditoria_DataHora",
                table: "LogAuditoria",
                column: "DataHora");

            migrationBuilder.CreateIndex(
                name: "IX_LogAuditoria_Entidade",
                table: "LogAuditoria",
                column: "Entidade");

            migrationBuilder.CreateIndex(
                name: "IX_LogAuditoria_TipoLogAuditoria",
                table: "LogAuditoria",
                column: "TipoLogAuditoria");

            migrationBuilder.CreateIndex(
                name: "IX_LogAuditoria_UsuarioId",
                table: "LogAuditoria",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Obra_ProjetoId",
                table: "Obra",
                column: "ProjetoId");

           

            migrationBuilder.CreateIndex(
                name: "IX_Obra_ResponsavelObraId",
                table: "Obra",
                column: "ResponsavelObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraDocumento_ObraId",
                table: "ObraDocumento",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraEquipamento_EquipamentoId",
                table: "ObraEquipamento",
                column: "EquipamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraEquipamento_EquipamentoId1",
                table: "ObraEquipamento",
                column: "EquipamentoId1");

            migrationBuilder.CreateIndex(
                name: "IX_ObraEquipamento_ObraId",
                table: "ObraEquipamento",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraEtapa_ObraId",
                table: "ObraEtapa",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraFornecedor_FornecedorId",
                table: "ObraFornecedor",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraFornecedor_ObraId",
                table: "ObraFornecedor",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraFuncionario_FuncionarioId",
                table: "ObraFuncionario",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraFuncionario_FuncionarioId1",
                table: "ObraFuncionario",
                column: "FuncionarioId1");

            migrationBuilder.CreateIndex(
                name: "IX_ObraFuncionario_ObraId",
                table: "ObraFuncionario",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraImagem_ObraId",
                table: "ObraImagem",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraInsumo_InsumoId",
                table: "ObraInsumo",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraInsumo_ObraId",
                table: "ObraInsumo",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraInsumo_ObraInsumoListaId",
                table: "ObraInsumo",
                column: "ObraInsumoListaId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraInsumoLista_ObraId",
                table: "ObraInsumoLista",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraInsumoLista_ResponsavelId",
                table: "ObraInsumoLista",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraItemEtapa_ObraEtapaId",
                table: "ObraItemEtapa",
                column: "ObraEtapaId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraItemEtapaPadrao_ObraEtapaPadraoId",
                table: "ObraItemEtapaPadrao",
                column: "ObraEtapaPadraoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraItemEtapaPadraoInsumo_InsumoId",
                table: "ObraItemEtapaPadraoInsumo",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraItemEtapaPadraoInsumo_ObraItemEtapaPadraoId",
                table: "ObraItemEtapaPadraoInsumo",
                column: "ObraItemEtapaPadraoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraPendencia_ObraId",
                table: "ObraPendencia",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraPendencia_ResponsavelId",
                table: "ObraPendencia",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraRetrabalho_ObraId",
                table: "ObraRetrabalho",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraRetrabalho_ResponsavelId",
                table: "ObraRetrabalho",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraServico_ObraId",
                table: "ObraServico",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraServico_ObraServicoListaId",
                table: "ObraServico",
                column: "ObraServicoListaId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraServico_ServicoId",
                table: "ObraServico",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraServicoLista_ObraId",
                table: "ObraServicoLista",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraServicoLista_ResponsavelId",
                table: "ObraServicoLista",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamento_ObraId",
                table: "Orcamento",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoItem_FornecedorId",
                table: "OrcamentoItem",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoItem_InsumoId",
                table: "OrcamentoItem",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoItem_OrcamentoId",
                table: "OrcamentoItem",
                column: "OrcamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoItem_OrcamentoObraId",
                table: "OrcamentoItem",
                column: "OrcamentoObraId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoItem_ServicoId",
                table: "OrcamentoItem",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoObra_ObraId_DataReferencia",
                table: "OrcamentoObra",
                columns: new[] { "ObraId", "DataReferencia" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantesEvento_EventoId",
                table: "ParticipantesEvento",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantesEvento_UsuarioId",
                table: "ParticipantesEvento",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoa_Email",
                table: "Pessoa",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoa_EnderecoId",
                table: "Pessoa",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_Projeto_ClienteId",
                table: "Projeto",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Projeto_EnderecoId",
                table: "Projeto",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_PerfilUsuarioId",
                table: "Usuario",
                column: "PerfilUsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracaoLembreteDiario");

            migrationBuilder.DropTable(
                name: "FornecedorInsumo");

            migrationBuilder.DropTable(
                name: "FornecedorServico");

            migrationBuilder.DropTable(
                name: "LembreteEvento");

            migrationBuilder.DropTable(
                name: "LogAuditoria");

            migrationBuilder.DropTable(
                name: "ObraDocumento");

            migrationBuilder.DropTable(
                name: "ObraEquipamento");

            migrationBuilder.DropTable(
                name: "ObraFornecedor");

            migrationBuilder.DropTable(
                name: "ObraFuncionario");

            migrationBuilder.DropTable(
                name: "ObraImagem");

            migrationBuilder.DropTable(
                name: "ObraInsumo");

            migrationBuilder.DropTable(
                name: "ObraItemEtapa");

            migrationBuilder.DropTable(
                name: "ObraItemEtapaPadraoInsumo");

            migrationBuilder.DropTable(
                name: "ObraPendencia");

            migrationBuilder.DropTable(
                name: "ObraRetrabalho");

            migrationBuilder.DropTable(
                name: "ObraServico");

            migrationBuilder.DropTable(
                name: "OrcamentoItem");

            migrationBuilder.DropTable(
                name: "ParticipantesEvento");

            migrationBuilder.DropTable(
                name: "Equipamento");

            migrationBuilder.DropTable(
                name: "ObraInsumoLista");

            migrationBuilder.DropTable(
                name: "ObraEtapa");

            migrationBuilder.DropTable(
                name: "ObraItemEtapaPadrao");

            migrationBuilder.DropTable(
                name: "ObraServicoLista");

            migrationBuilder.DropTable(
                name: "Fornecedor");

            migrationBuilder.DropTable(
                name: "Insumo");

            migrationBuilder.DropTable(
                name: "OrcamentoObra");

            migrationBuilder.DropTable(
                name: "Orcamento");

            migrationBuilder.DropTable(
                name: "Servico");

            migrationBuilder.DropTable(
                name: "Evento");

            migrationBuilder.DropTable(
                name: "ObraEtapaPadrao");

            migrationBuilder.DropTable(
                name: "Obra");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Funcionario");

            migrationBuilder.DropTable(
                name: "Projeto");

            migrationBuilder.DropTable(
                name: "PerfilUsuario");

            migrationBuilder.DropTable(
                name: "Funcao");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Pessoa");

            migrationBuilder.DropTable(
                name: "Endereco");
        }
    }
}
