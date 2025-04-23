using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;
    public CategoriaRepository(AppDbContext context)
    {
        _context = context;
    }
    public IEnumerable<Categoria> GetAll()
    {
        return _context.Categorias.ToList();
    }
    public Categoria Get(int id)
    {
        return _context.Categorias.Find(id);
    }
    public Categoria Create(Categoria categoria)
    {
        _context.Add(categoria);
        _context.SaveChanges();
        return categoria;
    }
    public Categoria Update(Categoria categoria)
    {
        _context.Entry(categoria).State = EntityState.Modified;
        _context.SaveChanges();
        return categoria;
    }
    public Categoria Delete(int id)
    {
        var categoria = _context.Categorias.Find(id);
        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
        return categoria;
    }
}
