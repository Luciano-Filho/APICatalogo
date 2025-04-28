using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _repository;
    public ProdutosController(IProdutoRepository repository)
    {
        _repository = repository;
    }
    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        try
        {
            var produtos = _repository.GetAll();
            if (!produtos.Any())
                return NoContent();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Ocorreu um erro na requisição: {ex.Message}");
        }
    }

    [HttpGet("{id:int}", Name= "ObterProdutoPorId")]
    public ActionResult<Produto> Get(int id)
    {
        var produto = _repository.Get(id);
        if (produto is null) return NotFound("Produto não encontrado...");
        return produto;
    }
    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        var produtoCriado = _repository.Create(produto);
        return new CreatedAtRouteResult("ObterProdutoPorId",new {id = produtoCriado.Id}, produtoCriado);
    }
    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.Id)
            return BadRequest("O id informado deve ser igual ao id do produto a ser atualizado");

        if(_repository.Update(produto))
            return Ok(produto);
        return BadRequest("Produto não encontrado");
    }
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var produto = _repository.Get(id);
        if (produto is null) return NotFound("Produto não encontrado...");

        if(_repository.Delete(produto))
            return Ok(produto);
        return BadRequest("Produto não deletado");
    }
}
