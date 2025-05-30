﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using APICatalogo.Models;
using APICatalogo.Models.DTOs;
using APICatalogo.Services;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy="SuperAdminOnly")]
    [HttpPost]
    [Route("CreateRole")]
    public async Task<ActionResult> CreateRole(string roleName)
    {
        var roleExiste = await _roleManager.RoleExistsAsync(roleName);
        if(!roleExiste)
        {
            var resultado = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (resultado.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK, new Response 
                { Status = "Sucesso", Message = $"Role {roleName} criada com sucesso" });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                {
                    Status = "Erro",Message = $"Falha na criação da role {roleName}"
                });
            }
        }
        else
        {
            return StatusCode(StatusCodes.Status400BadRequest, new Response
            {
                Status = "400",
                Message = $"Role {roleName} já existe"
            });
        }
    }
    [HttpPost]
    [Route("AdicionaUserARole")]
    public async Task<ActionResult> AdicionaUsuarioARole(string email, string role)
    {
        var usuario = await _userManager.FindByEmailAsync(email);
        if(usuario != null)
        {
            var resultado = await _userManager.AddToRoleAsync(usuario, role);
            if(resultado.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK, new Response
                {
                    Status = "Sucesso",
                    Message = $"Usuário {usuario.UserName} adicionado a role {role}"
                });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                {
                    Status = "Falha",
                    Message = $"Ocorreu uma falha ao atribuir o usuario {usuario.UserName} a role {role}"
                });
            }
        }
        return BadRequest(new { Error = "Usuario não encontrado" });
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> login(LoginModel loginModel)
    {
        var usuario = await _userManager.FindByNameAsync(loginModel.UserName);
        if ((usuario is not null && await _userManager.CheckPasswordAsync(usuario,loginModel.Password!)))
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,usuario.UserName!),
                new Claim(ClaimTypes.Email, usuario.Email!),
                new Claim("id", usuario.UserName!),
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
    [HttpPost]
    [Route("Registro")]
    public async Task<ActionResult> RegistrarUsuario(RegisterModel model)
    {
        var usuarioExiste = await _userManager.FindByNameAsync(model.UserName!); //exclamação para dizer ao compilador que não é nulo
        if (usuarioExiste != null)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Erro", Message = "usuário já existe" });

        ApplicationUser usuario = new()
        {
            UserName = model.UserName,
            Email = model.EmailAddress,
            SecurityStamp = Guid.NewGuid().ToString(),//para criar um novo guid para o usuario
        };
        var resultado = await _userManager.CreateAsync(usuario, model.Password!);
        if (!resultado.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Erro", Message = "Falha na criação do usuário" });

        return Ok(new Response { Status="Sucesso", Message="Usuário criado com sucesso"});
    }
    [HttpPost]
    [Route("refresh-token")]
    public async Task<ActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel == null)
            return BadRequest("Requisição inválida");

        string? tokenAcesso = tokenModel.TokenAcesso
                ?? throw new ArgumentNullException(nameof(tokenModel));
        string? refreshToken = tokenModel.TokenAtualizado
                ?? throw new ArgumentNullException(nameof(refreshToken));

        var principal = _tokenService.GetClaimsPrincipalFromExpiredToken(tokenAcesso, _configuration);

        if (principal is null)
            return BadRequest("token ou refresh token é inválido!");

        var userName = principal.Identity.Name;
        var usuario = await _userManager.FindByNameAsync(userName!);

        if(usuario == null || usuario.RefreshToken != refreshToken ||
                                usuario.RefreshTokenExpiryTime <= DateTime.Now)
            return BadRequest("token ou refresh token é inválido!");

        var novoTokenAcesso = _tokenService.GerarTokenAcesso(principal.Claims.ToList(), _configuration);
        var novoRefrehToken = _tokenService.AtualizarToken();

        usuario.RefreshToken = novoRefrehToken;
        
        await _userManager.UpdateAsync(usuario);

        return new ObjectResult(new
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(novoTokenAcesso),
            RefreshToken = novoRefrehToken
        });
    }
    [Authorize]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<ActionResult> RevokeToken(string username)
    {
        var usuario = await _userManager.FindByNameAsync(username);
        if (usuario is null) return BadRequest("username é inválido");

        usuario.RefreshToken = null;
        await _userManager.UpdateAsync(usuario);

        return NoContent();
    }
}
