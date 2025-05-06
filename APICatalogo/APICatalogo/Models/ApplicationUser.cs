using Microsoft.AspNetCore.Identity;

namespace APICatalogo.Models;
//Classe criada para adicionar dois campos na tabela de usuarios do identity
public class ApplicationUser :IdentityUser
{
    public string? RefreshToken {  get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
