using Application.CryptoFacilBrasil.BasicAuthentication;
using CryptoFacilBrasil.Domain.IRepository;
using CryptoFacilBrasil.Domain.Settings;
using CryptoFacilBrasil.Services;
using CryptoFacilBrasil.Services.AuthBot;
using CryptoFacilBrasil.Services.Interfaces.Services;
using Infrastructure.Entityframework;
using Infrastructure.Entityframework.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionString_OAuth_dev")));

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();


builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();




builder.Services.AddControllersWithViews();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddHttpClient<RequestManagement>()
    .ConfigureHttpClient((serviceProvider, client) =>
    {
        // Obtenha o token de autenticação do arquivo de configuração (appsettings.json ou outra fonte)
        var authToken = builder.Configuration["AuthenticationMyBot"];

        // Verifique se o token foi obtido corretamente
        if (string.IsNullOrEmpty(authToken))
        {
            throw new InvalidOperationException("Authentication token is missing in configuration.");
        }

        // Adiciona o cabeçalho de Autorização ao cliente HTTP
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
    });



// Configuração do cliente do bot com o token de forma segura
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(builder.Configuration["TelegramBotToken"]));
builder.Services.AddHostedService<TelegramBotBackgroundService>();

builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("AuthenticationMyBot"));


builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BasicUser", policy => policy.RequireAuthenticatedUser());
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


// Mapeando as rotas do controlador
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
