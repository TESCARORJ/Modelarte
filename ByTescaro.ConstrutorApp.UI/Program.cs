using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Application.Mappings;
using ByTescaro.ConstrutorApp.Application.Services;
using ByTescaro.ConstrutorApp.Domain.Interfaces.ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using ByTescaro.ConstrutorApp.UI.Authentication;
using ByTescaro.ConstrutorApp.UI.Components;
using ByTescaro.ConstrutorApp.UI.Properties;
using ByTescaro.ConstrutorApp.UI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte ao Radzen ThemeService
builder.Services.AddScoped<Radzen.ThemeService>();
builder.Services.AddScoped<Radzen.ThemeService>();
builder.Services.AddScoped<Radzen.DialogService>();
builder.Services.AddScoped<Radzen.NotificationService>();
builder.Services.AddScoped<Radzen.TooltipService>();
builder.Services.AddScoped<Radzen.ContextMenuService>();


// 🔐 Persistência de chaves compartilhada entre subdomínios
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\DataProtectionKeys\ConstrutorApp"))
    .SetApplicationName("ConstrutorApp");

// 🍪 Configuração de cookie com domínio compartilhado (.construtorapp.com)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".ConstrutorApp.Auth";
    options.Cookie.Domain = ".construtorapp.com"; // funciona para qualquer subdomínio
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.LoginPath = "/login"; // ajuste se necessário
    options.LogoutPath = "/logout";
    options.SlidingExpiration = true;
});


#region [ Blazor + MudBlazor ]


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthCookieService>();

#endregion

#region [ Database Context ]

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

#endregion

#region [ Autenticação com Cookie Customizado ]

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/acesso-negado";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = false;
    });

builder.Services.AddAuthorization();

#endregion

#region [ Application Services ]


builder.Services.AddHttpClient<CepService>();
builder.Services.AddScoped<IUsuarioLogadoService, UsuarioLogadoService>();
builder.Services.AddScoped<IPerfilUsuarioService, PerfilUsuarioService>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ILogAuditoriaService, LogAuditoriaService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProjetoService, ProjetoService>();
builder.Services.AddScoped<IInsumoService, InsumoService>();
builder.Services.AddScoped<IEquipamentoService, EquipamentoService>();
builder.Services.AddScoped<IFuncionarioService, FuncionarioService>();
builder.Services.AddScoped<IObraService, ObraService>();
builder.Services.AddScoped<IObraEtapaPadraoService, ObraEtapaPadraoService>();
builder.Services.AddScoped<IObraItemEtapaPadraoService, ObraItemEtapaPadraoService>();
builder.Services.AddScoped<IObraEtapaService, ObraEtapaService>();
builder.Services.AddScoped<IObraItemEtapaService, ObraItemEtapaService>(); 
builder.Services.AddScoped<IObraEquipamentoService, ObraEquipamentoService>();
builder.Services.AddScoped<IObraFuncionarioService, ObraFuncionarioService>();
builder.Services.AddScoped<IObraInsumoService, ObraInsumoService>();
builder.Services.AddScoped<IObraInsumoListaService, ObraInsumoListaService>();
builder.Services.AddScoped<IFuncaoService, FuncaoService>();
builder.Services.AddScoped<IClienteImportacaoService, ClienteImportacaoService>();
builder.Services.AddScoped<IObraChecklistService, ObraChecklistService>();
builder.Services.AddScoped<IObraRetrabalhoService, ObraRetrabalhoService>();
builder.Services.AddScoped<IObraPendenciaService, ObraPendenciaService>();



builder.Services.AddAutoMapper(typeof(ApplicationProfile));

#endregion


#region [ API Consumers - ApiService ]


var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl não configurada");

builder.Services.AddScoped<PerfilUsuarioApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new PerfilUsuarioApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});

builder.Services.AddScoped<UsuarioApiService>(sp =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new UsuarioApiService(sp.CreateHttpClientWithCookies(baseUrl));
});


builder.Services.AddScoped<ClienteApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ClienteApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});


builder.Services.AddScoped<ProjetoApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ProjetoApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});



builder.Services.AddScoped<EquipamentoApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new EquipamentoApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});

builder.Services.AddScoped<FuncionarioApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new FuncionarioApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});

builder.Services.AddScoped<InsumoApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new InsumoApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});

builder.Services.AddScoped<ObraApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ObraApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});



builder.Services.AddScoped<ObraEtapaPadraoApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ObraEtapaPadraoApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});


builder.Services.AddScoped<ObraItemEtapaPadraoApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ObraItemEtapaPadraoApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});


builder.Services.AddScoped<ObraFuncionarioApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ObraFuncionarioApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});

builder.Services.AddScoped<ObraEquipamentoApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ObraEquipamentoApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});

builder.Services.AddScoped<ObraInsumoApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ObraInsumoApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});


builder.Services.AddScoped<ObraInsumoListaApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ObraInsumoListaApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});

builder.Services.AddScoped<FuncaoApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new FuncaoApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});

builder.Services.AddScoped<ClienteImportacaoApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ClienteImportacaoApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});


builder.Services.AddScoped<ObraChecklistApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ObraChecklistApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});

builder.Services.AddScoped<ObraRetrabalhoApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ObraRetrabalhoApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});



builder.Services.AddScoped<ObraPendenciaApiService>(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    return new ObraPendenciaApiService(sp.CreateHttpClientWithCookies(apiBaseUrl));
});



#endregion


#region [ Repository ]

builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
builder.Services.AddScoped<IEquipamentoRepository, EquipamentoRepository>();
builder.Services.AddScoped<IInsumoRepository, InsumoRepository>();
builder.Services.AddScoped<ILogAuditoriaRepository, LogAuditoriaRepository>();
builder.Services.AddScoped<IPerfilUsuarioRepository, PerfilUsuarioRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IFuncaoRepository, FuncaoRepository>();
builder.Services.AddScoped<IObraRepository, ObraRepository>();
builder.Services.AddScoped<IObraEtapaPadraoRepository, ObraEtapaPadraoRepository>();
builder.Services.AddScoped<IObraItemEtapaPadraoRepository, ObraItemEtapaPadraoRepository>();
builder.Services.AddScoped<IObraEtapaRepository, ObraEtapaRepository>();
builder.Services.AddScoped<IObraItemEtapaRepository, ObraItemEtapaRepository>();
builder.Services.AddScoped<IObraFuncionarioRepository, ObraFuncionarioRepository>();
builder.Services.AddScoped<IObraEquipamentoRepository, ObraEquipamentoRepository>();
builder.Services.AddScoped<IObraInsumoRepository, ObraInsumoRepository>();
builder.Services.AddScoped<IObraInsumoListaRepository, ObraInsumoListaRepository>();
builder.Services.AddScoped<IObraRetrabalhoRepository, ObraRetrabalhoRepository>();
builder.Services.AddScoped<IObraPendenciaRepository, ObraPendenciaRepository>();
builder.Services.AddScoped<IObraDocumentoRepository, ObraDocumentoRepository>();
builder.Services.AddScoped<IObraImagemRepository, ObraImagemRepository>();


#endregion



builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    opt.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddHttpContextAccessor();


#region [ Build & Configure App ]

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Mapeamento da API
app.MapControllers();

// Mapeamento da UI Blazor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await DbSeeder.SeedAdminAsync(app.Services);

app.Run();

#endregion
