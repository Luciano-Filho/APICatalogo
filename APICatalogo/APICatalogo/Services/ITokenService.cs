using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICatalogo.Services;

public interface ITokenService
{
    //Gera o token de acesso
    JwtSecurityToken GerarTokenAcesso(IEnumerable<Claim> claims,IConfiguration _config);
    //Atualiza o token de acesso
    string AtualizarToken();
    //Extrai as claims do token expirado
    ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token, IConfiguration _config);
}
