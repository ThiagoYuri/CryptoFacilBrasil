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
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionString_OAuth_dev")));


// Configura��o do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3001", builder =>
    {
        builder.WithOrigins("http://localhost:3002")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // Permite envio de cookies, se necess�rio
    });
});

// Reposit�rios
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

// Servi�os
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();

// Configura��o dos controladores
builder.Services.AddControllersWithViews();

// Configura��o de API Settings
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Configura��o de HTTP Client
builder.Services.AddHttpClient<RequestManagement>()
    .ConfigureHttpClient((serviceProvider, client) =>
    {
        var authToken = builder.Configuration["AuthenticationMyBot"];
        if (string.IsNullOrEmpty(authToken))
        {
            throw new InvalidOperationException("Authentication token is missing in configuration.");
        }
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
    });

// Configura��o do Bot do Telegram
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(builder.Configuration["TelegramBotToken"]));
builder.Services.AddHostedService<TelegramBotBackgroundService>();

// Configura��o de Autentica��o
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

// Configura��o de Autoriza��o
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes("BasicAuthentication")
        .Build();
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});


// Configura��o do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("BasicAuthentication", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Basic Authentication using username and password",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "BasicAuthentication"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Substitua pela URL do seu frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();


app.UseCors();
// Configura��o do CORS no pipeline de middleware
app.UseCors("AllowLocalhost3001");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
});

// Prote��o das rotas do Swagger
app.Map("/api-documentacao", subApp =>
{
    subApp.UseSwagger();
    subApp.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = string.Empty;
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");

    // Adiciona o bot�o "Authorize" no Swagger UI
    c.DisplayOperationId();
    c.DisplayRequestDuration();
});



// Configura��es padr�o de middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Controladores protegidos
app.MapControllers();






app.Run();
