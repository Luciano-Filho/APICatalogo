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
    private readonly IUnitOfWork _iof;
    public ProdutosController(IUnitOfWork iof)
    {
        _iof = iof;
    }
    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        try
        {
            var produtos = _iof.ProdutoRepository.GetAll();
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
        var produto = _iof.ProdutoRepository.Get(id);
        if (produto is null) return NotFound("Produto não encontrado...");
        return produto;
    }
    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        var produtoCriado = _iof.ProdutoRepository.Create(produto);
        _iof.Commit();
        return new CreatedAtRouteResult("ObterProdutoPorId",new {id = produtoCriado.Id}, produtoCriado);
    }
    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.Id)
            return BadRequest("O id informado deve ser igual ao id do produto a ser atualizado");

        if(_iof.ProdutoRepository.Update(produto))
            _iof.Commit();
            return Ok(produto);
        return BadRequest("Produto não encontrado");
    }
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var produto = _iof.ProdutoRepository.Get(id);
        if (produto is null) return NotFound("Produto não encontrado...");

        if (_iof.ProdutoRepository.Delete(produto))
            _iof.Commit();
            return Ok(produto);
        return BadRequest("Produto não deletado");
    }
}
