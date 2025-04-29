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
    private readonly IUnitOfWork _iof;
    string caminhoLog = "C:\\Users\\Luciano\\Documents\\APIs\\Desenvolvimento\\Logs";
    public CategoriasController(IUnitOfWork iof)
    {
        _iof = iof;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> GetAll()
    {
        var categorias = _iof.CategoriaRepository.GetAll();
        if (!categorias.Any())
            return NotFound();
        return Ok(categorias);
    }

    [HttpGet("{id:int}", Name = "CategoriaPorId")]
    public ActionResult<Categoria> Get(int id)
    {
        var categoria = _iof.CategoriaRepository.Get(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada...");
        return Ok(categoria);
    }
    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
        var categoriaCriada = _iof.CategoriaRepository.Create(categoria);
        _iof.Commit();
        return new CreatedAtRouteResult("CategoriaPorId", new { id = categoriaCriada.Id }, categoriaCriada);
    }
    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Categoria categoria)
    {
        if (id != categoria.Id)
            return BadRequest("O id informado deve ser o mesmo id da categoria");
        var categoriaExistente = _iof.CategoriaRepository.Get(id);
        if (categoriaExistente is null)
            return NotFound("Não há categoria para o id informado");

        _iof.CategoriaRepository.Update(categoria);
        _iof.Commit();
        return Ok(categoria);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _iof.CategoriaRepository.Get(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada...");
        _iof.CategoriaRepository.Delete(id);
        _iof.Commit();
        return Ok(categoria);
    }

}
