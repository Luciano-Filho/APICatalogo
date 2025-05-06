using System.ComponentModel.DataAnnotations;

namespace APICatalogo.Models.DTOs;

public class LoginModel
{
    [Required(ErrorMessage ="Username é obrigatório")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Password é obrigatório")]
    public string? Password { get; set; }
}
