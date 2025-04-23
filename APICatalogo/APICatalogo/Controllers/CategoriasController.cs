using System.Text;
using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaRepository _Repository;
    string caminhoLog = "C:\\Users\\Luciano\\Documents\\APIs\\Desenvolvimento\\Logs";
    public CategoriasController(ICategoriaRepository Repository)
    {
        _Repository = Repository;
    }
    
    /*[HttpGet("produtos")]
    public ActionResult<IEnumerable<Categoria>> GetGategoriaProduto()
    {
        StreamWriter log = new StreamWriter(caminhoLog + "/Categorias.txt", true);
        log.Write("Imprimindo todas as categorias");
        var categorias = _Repository.GetAll();
        return Ok(categorias);
    }*/
    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> GetAll()
    {
        var categorias = _Repository.GetAll();
        return Ok(categorias);
    }

    [HttpGet("{id:int}", Name = "CategoriaPorId")]
    public ActionResult<Categoria> Get(int id)
    {
        var categoria = _Repository.Get(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada...");
        return Ok(categoria);
    }
    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
        var categoriaCriada = _Repository.Create(categoria);
        return new CreatedAtRouteResult("CategoriaPorId", new { id = categoriaCriada.Id }, categoriaCriada);
    }
    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Categoria categoria)
    {
        if (id != categoria.Id)
            return BadRequest("O id informado deve ser o mesmo id da categoria");

        var categoriaAtualizada = _Repository.Update(categoria);
        return Ok(categoriaAtualizada);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _Repository.Get(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada...");
        _Repository.Delete(id);
        return Ok(categoria);
    }

}
