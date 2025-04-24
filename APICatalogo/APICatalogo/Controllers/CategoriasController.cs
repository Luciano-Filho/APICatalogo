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
    
    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> GetAll()
    {
        var categorias = _Repository.GetAll();
        if (!categorias.Any())
            return NotFound();
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
        //var categoriaExistente = _Repository.Get(id);
        //if (categoriaExistente is null)
          //  return NotFound("Não há categoria para o id informado");

        _Repository.Update(categoria);
        return Ok(categoria);
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
