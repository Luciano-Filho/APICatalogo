using System.ComponentModel.DataAnnotations;

namespace APICatalogo.Models.DTOs;

public class RegisterModel
{
    [Required(ErrorMessage = "Username é obrigatório")]
    public string? UserName { get; set; }
    [EmailAddress]
    [Required(ErrorMessage = "Email é obrigatório")]
    public string? EmailAddress { get; set; }
    [Required(ErrorMessage = "Password é obrigatório")]
    public string? Password { get; set; }
}
