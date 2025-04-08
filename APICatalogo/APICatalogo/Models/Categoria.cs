using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APICatalogo.Models;

public class Categoria
{
    public int Id { get; set; }
    [Required]
    [StringLength(80)]
    public string? Nome { get; set; }
    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>(); 
}
