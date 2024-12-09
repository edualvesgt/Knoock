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
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Adicione os serviços necessários
builder.Services
    .AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        });

builder.Services.AddDbContext<KnoockContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KnoockDatabase")));

//builder.Services.AddSignalR();
//.AddAzureSignalR(builder.Configuration["SignalR:ConnectionStringSignalR"]);

builder.Services.AddSignalR(options => options.EnableDetailedErrors = true);


//Adiciona serviço de Jwt Bearer (forma de autenticação)
builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = "JwtBearer";
    options.DefaultAuthenticateScheme = "JwtBearer";
})

.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //valida quem está solicitando
        ValidateIssuer = true,

        //valida quem está recebendo
        ValidateAudience = true,

        //define se o tempo de expiração será validado
        ValidateLifetime = true,

        //forma de criptografia e valida a chave de autenticação
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("knoock-webapi-chave-symmetricsecuritykey")),

        //valida o tempo de expiração do token
        ClockSkew = TimeSpan.FromMinutes(30),

        //nome do issuer (de onde está vindo)
        ValidIssuer = "knoock-WebAPI",

        //nome do audience (para onde está indo)
        ValidAudience = "knoock-WebAPI"
    };
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Knoock API",
        Description = "API do KnoockApp",
        //TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Grupo Knoock",
            Email = string.Empty,
            //Url = new Uri("https://twitter.com/spboyer"),
        },
        License = new OpenApiLicense
        {
            //Name = "Use under LICX",
            //Url = new Uri("https://example.com/license"),
        }
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);



    //Usando a autenticaçao no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Value: Bearer TokenJWT ",
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


//ADICIONANDO NO ESCOPO DO PROJETO

builder.Services.AddScoped<ISmsRepository, SmsRepository>(); // Repositório de SMS
builder.Services.AddScoped<SmsService>(); // Serviço de envio de SMS
// Adicionar serviços SignalR
builder.Services.AddSignalR();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));
// Registrar o serviço de envio de e-mails
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<EmailSendingService>();
builder.Services.AddScoped<IAfiliadosRepository, AfiliadosRepository>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEntregaRepository, EntregaRepository>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();


// Adicionar política CORS no serviço
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
    });
});



var app = builder.Build();

//Alterar dados do Swagger para a seguinte configuração
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwaggerUI();


//Importante para o deploy
//Para atender à interface do usuário do Swagger na raiz do aplicativo
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseCors("CorsPolicy");


app.UseHttpsRedirection();
app.UseAuthentication();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<EntregasHub>("/entregashub");
    endpoints.MapControllers();
});

//app.UseAuthorization();
app.MapControllers();

app.Run();
