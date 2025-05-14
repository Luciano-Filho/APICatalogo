
using System.Text;
using System.Threading.RateLimiting;
using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Models.DTOs.Mappings;
using APICatalogo.Repositories;
using APICatalogo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };
});

//A ordem importa: UseAuthentication vem antes de UseAuthorization.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("Admin").
                                                RequireClaim("id", "luciano"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("ExclusivePolicyOnly", policy => policy.RequireAssertion(context =>
                                      context.User.HasClaim(Claim => Claim.Type == "id" &&
                                       Claim.Value == "luciano") || context.User.IsInRole("SuperAdmin")));
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "WindowsLimit", limit =>
    {
        limit.PermitLimit = 1; //Quantidade de requisições dentro do tempo estabelecido
        limit.Window = TimeSpan.FromSeconds(5); //Intervalo de tempo
        limit.QueueLimit = 0; //Requisições que vão ficar na fila
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

//Adiconar limite de taxa para toda a aplicação
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
    RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpcontext.User.Identity?.Name ??
                                httpcontext.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = bool.Parse(builder.Configuration["LimitRateValues:AutoReplenishment"]!),
                    PermitLimit = int.Parse(builder.Configuration["LimitRateValues:PermitLimit"]!),
                    QueueLimit = int.Parse(builder.Configuration["LimitRateValues:QueueLimit"]!),
                    Window = TimeSpan.FromSeconds(int.Parse(builder.Configuration["LimitRateValues:Window"]!))
                }));
});

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
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization(); 
app.UseRateLimiter();//Deve ser inserido depois do userRouting 
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();
