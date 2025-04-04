using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;
    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet("produtos")]
    public ActionResult<IEnumerable<Categoria>> GetGategoriaProduto()
    {
        return _context.Categorias.Include(p=> p.Produtos).ToList();
    }
    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> GetAll()
    {
        return _context.Categorias.ToList();
    }

    [HttpGet("{id:int}", Name = "CategoriaPorId")]
    public ActionResult<Categoria> Get(int id)
    {
        var categoria = _context.Categorias.Find(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada...");
        return Ok(categoria);
    }
    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        _context.SaveChanges();
        return new CreatedAtRouteResult("CategoriaPorId", new { id = categoria.Id }, categoria);
    }
    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Categoria categoria)
    {
        if (id != categoria.Id)
            return BadRequest("O id informado deve ser o mesmo id da categoria");

        _context.Entry(categoria).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _context.SaveChanges();
        return Ok(categoria);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _context.Categorias.Find(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada...");
        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
        return Ok(categoria);
    }

}
