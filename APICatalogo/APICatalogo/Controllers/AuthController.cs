using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using APICatalogo.Models;
using APICatalogo.Models.DTOs;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(ITokenService tokenService,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager, 
                IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> login(LoginModel loginModel)
    {
        var usuario = await _userManager.FindByNameAsync(loginModel.UserName);
        if ((usuario is not null && await _userManager.CheckPasswordAsync(usuario,loginModel.Password)))
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,usuario.UserName),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var token = _tokenService.GerarTokenAcesso(authClaims, _configuration);
            var refreshToken = _tokenService.AtualizarToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"],
                out int validadeRefreshToken); //tryParse retorna um bool, então nao precisa atribuir a uma variavel

            usuario.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(validadeRefreshToken);
            usuario.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(usuario);

            //retorna um json
            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }
}
