
using APICatalogo.Context;
using APICatalogo.Models.DTOs.Mappings;
using APICatalogo.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddOpenApi();//v1.json
builder.Services.AddScoped<ICategoriaRepository,CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository,ProdutoRepository>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddAutoMapper(typeof(ProdutoDtoMappingProfile));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
