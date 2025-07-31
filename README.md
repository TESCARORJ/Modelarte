# ConstrutorApp: Sistema de Gerenciamento de Obras

## Descrição do Projeto

O ConstrutorApp é uma aplicação web robusta, desenvolvida em .NET Core e Blazor, projetada para o gerenciamento completo de obras de construção. A plataforma centraliza o controle de recursos, equipes e documentos, permitindo um acompanhamento eficiente de cada projeto.

## Funcionalidades Principais

* **Gerenciamento de Obras**: CRUD (Criação, Leitura, Atualização, Exclusão) completo para projetos de construção.
* **Gestão de Equipe**: Alocação de funcionários por obra, com controle de função, data de início e término.
* **Controle de Documentos**: Upload, listagem, visualização e exclusão de documentos, com suporte a compressão e armazenamento seguro.
* **Galeria de Imagens**: Upload, visualização em galeria e exclusão de fotos da obra.
* **Notificações por WhatsApp**: Integração com a API Z-API para envio de relatórios detalhados das obras para usuários e grupos.
* **Gerenciamento de Recursos**: Controle de checklists, equipamentos, insumos, serviços, retrabalhos e pendências por obra.
* **Relatórios em PDF**: Geração de relatórios completos da obra em formato PDF.

## Tecnologias e Ferramentas

O projeto segue uma arquitetura modular, com uma separação clara de responsabilidades, utilizando as seguintes tecnologias:

* **Frontend**:
    * **Blazor WebAssembly**: Framework .NET para construção da interface de usuário interativa no navegador.
    * **Radzen Blazor Components**: Conjunto de componentes de UI modernos e responsivos.
* **Backend**:
    * **.NET 7/8 (C#)**: Plataforma principal de desenvolvimento.
    * **ASP.NET Core**: Para os serviços de API que se comunicam com o frontend.
    * **Entity Framework Core**: ORM para acesso e manipulação de dados no banco de dados.
* **Serviços e APIs**:
    * **AutoMapper**: Para mapeamento de objetos entre entidades e DTOs (Data Transfer Objects).
    * **RestSharp**: Cliente HTTP para consumir a API do Z-API.
    * **API Z-API**: Plataforma de terceiros para integração com o WhatsApp.

## Estrutura do Projeto (Arquitetura)

A aplicação é dividida em camadas lógicas para garantir escalabilidade e manutenção:

* `ByTescaro.ConstrutorApp.Domain`: Contém as entidades de negócio (`ObraFuncionario`, `ObraDocumento`, `ObraImagem`), interfaces de repositório e enums. Representa o núcleo do negócio.
* `ByTescaro.ConstrutorApp.Application`: Camada de aplicação que define os DTOs (`ObraFuncionarioDto`, `ObraDocumentoDto`, etc.) e a lógica de negócios através de serviços (`IObraFuncionarioService`, `IObraDocumentoService`).
* `ByTescaro.ConstrutorApp.Infrastructure`: Contém as implementações concretas das interfaces do `Domain`, como os repositórios (`ObraFuncionarioRepository`) e o `DbContext` para o Entity Framework Core.
* `ByTescaro.ConstrutorApp.UI`: A camada de apresentação (Frontend), que inclui os componentes Blazor (`.razor`), os serviços de API (`ObraFuncionarioApiService`), os controllers para o backend (`ObraFuncionarioController`) e a configuração da aplicação.

## Configuração e Instalação

Siga os passos abaixo para configurar e executar o projeto em seu ambiente de desenvolvimento.

### Pré-requisitos

* [.NET SDK](https://dotnet.microsoft.com/download) (versão 7 ou superior)
* [Visual Studio](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)
* Banco de dados compatível com Entity Framework Core (ex: SQL Server, SQLite, PostgreSQL)
* Chaves de acesso para a API do Z-API (se for usar a funcionalidade de notificações)

### Passos

1.  **Clonar o repositório**:
    ```bash
    git clone [https://github.com/SeuUsuario/SeuRepositorio.git](https://github.com/SeuUsuario/SeuRepositorio.git)
    cd SeuRepositorio
    ```

2.  **Configurar o Banco de Dados**:
    * Abra o arquivo `appsettings.json` na pasta `ByTescaro.ConstrutorApp.UI`.
    * Atualize a string de conexão (`"ConnectionStrings:DefaultConnection"`) para o seu banco de dados.

3.  **Configurar a API Z-API (Opcional)**:
    * No mesmo arquivo `appsettings.json`, adicione as suas chaves da API do Z-API:
    ```json
    "ZApi": {
      "BaseUrl": "[https://api.z-api.io](https://api.z-api.io)",
      "InstanceId": "SUA_INSTANCIA",
      "InstanceToken": "SEU_TOKEN",
      "ClientToken": "SEU_TOKEN_DE_SEGURANCA"
    }
    ```

4.  **Restaurar pacotes NuGet**:
    ```bash
    dotnet restore
    ```

5.  **Executar Migrações do Banco de Dados**:
    * Navegue até o projeto `ByTescaro.ConstrutorApp.Infrastructure`.
    * Execute os seguintes comandos para criar o banco de dados e as tabelas:
    ```bash
    dotnet ef database update
    ```

6.  **Executar o Projeto**:
    * Navegue de volta para a pasta do projeto `ByTescaro.ConstrutorApp.UI`.
    * Execute a aplicação:
    ```bash
    dotnet run
    ```
    * A aplicação estará disponível em `https://localhost:7080` (ou na porta configurada).
