
using System.Text;
using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Models.DTOs.Mappings;
using APICatalogo.Repositories;
using APICatalogo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddOpenApi();//v1.json
builder.Services.AddScoped<ICategoriaRepository,CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository,ProdutoRepository>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAutoMapper(typeof(ProdutoDtoMappingProfile));
//linha baixo foi inserida para tratar dos perfis de usuario
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var secretKey = builder.Configuration["JWT:SecretKey"] ?? throw new ArgumentException("Chave secreta invalida...");

builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,//Verifica se o token veio de um emissor confiável
        ValidateAudience = true,//Verifica se o token está sendo usado pela aplicação certa
        ValidateLifetime = true, //Garante que o token não esteja expirado
        ValidateIssuerSigningKey = true, //Verifica se o token foi assinado corretamente.
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"], //Quem emitiu o token (precisa bater com o que foi usado ao gerar o token)
        ValidAudience = builder.Configuration["JWT:ValidAudience"],//Para quem o token é válido (normalmente o próprio serviço)
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))

    };
});
//A ordem importa: UseAuthentication vem antes de UseAuthorization.
builder.Services.AddAuthorization();

string connectionString = builder.Configuration.GetConnectionString("defaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "API Catalogo"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization(); 
app.UseExceptionHandler("/erro"); // ou um middleware customizado
app.MapControllers();

app.Run();
