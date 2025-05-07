using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace APICatalogo.Services;

public class TokenService : ITokenService
{
    public JwtSecurityToken GerarTokenAcesso(IEnumerable<Claim> claims, IConfiguration _config)
    {
        var key = _config.GetSection("JWT").GetValue<string>("SecretKey") ??
            throw new InvalidOperationException("Chave secreta inválida");
        var secretKey = Encoding.UTF8.GetBytes(key);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
                                    SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_config.GetSection("JWT").
                                            GetValue<double>("TokenValidityInMinutes")),
            Audience = _config.GetSection("JWT").GetValue<string>("ValidAudience"),
            Issuer = _config.GetSection("JWT").GetValue<string>("ValidIssuer"),
            SigningCredentials = signingCredentials
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return token;
    }
    public string AtualizarToken()
    {
        var bytesAleatorios = new byte[128];
        using var geradorNumerosAleatorios = RandomNumberGenerator.Create();
        geradorNumerosAleatorios.GetBytes(bytesAleatorios);
        var tokenAtualizado = Convert.ToBase64String(bytesAleatorios);
        return tokenAtualizado;
    }
    public ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token, IConfiguration _config)
    {
        var key = _config["JWT:SecretKey"] ?? throw new InvalidOperationException("Chave secreta invalida");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                                out SecurityToken securityToken);
        if(securityToken is not JwtSecurityToken jwtSecurityToken ||
                            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                            StringComparison.InvariantCultureIgnoreCase))
        {
            throw new InvalidOperationException("token invalido");
        }
        return principal;
    }
}
