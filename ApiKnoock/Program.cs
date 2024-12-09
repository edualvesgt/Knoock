using ApiKnoock.Context;
using ApiKnoock.Domains;
using ApiKnoock.Interface;
using ApiKnoock.Repository;
using ApiKnoock.Utils.Email;
using ApiKnoock.Utils.Sms;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços necessários
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });

builder.Services.AddDbContext<KnoockContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KnoockDatabase")));

// Adicionar SignalR com suporte detalhado
builder.Services.AddSignalR(options => options.EnableDetailedErrors = true);

// Configuração JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    options.Authority = "https://exemple.com";
    options.Audience = "api";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes("knoock-webapi-chave-symmetricsecuritykey")),
        ValidIssuer = "knoock-WebAPI",
        ValidAudience = "knoock-WebAPI"
    };

    // Permitir envio do token ao SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/entregashub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Erro de autenticação: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Token validado para o usuário: {context.Principal.Identity.Name}");
            return Task.CompletedTask;
        }
    };
});

// Adicionar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Knoock API",
        Description = "API do KnoockApp",
        Contact = new OpenApiContact
        {
            Name = "Grupo Knoock",
            Email = string.Empty
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Value: Bearer TokenJWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Adicionar repositórios e serviços ao escopo do projeto
builder.Services.AddScoped<ISmsRepository, SmsRepository>();
builder.Services.AddScoped<SmsService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<EmailSendingService>();
builder.Services.AddScoped<IAfiliadosRepository, AfiliadosRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEntregaRepository, EntregaRepository>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:5109")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger(options => options.SerializeAsV2 = true);
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

// Garantir que o middleware de autenticação e autorização está configurado
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

// Configuração de endpoints para SignalR e controllers
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<EntregasHub>("/entregashub");
    endpoints.MapControllers();
});

app.Run();
