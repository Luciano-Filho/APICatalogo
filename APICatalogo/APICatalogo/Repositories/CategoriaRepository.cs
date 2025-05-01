using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;
    public CategoriaRepository(AppDbContext context)
    {
        _context = context;
    }
    public IEnumerable<Categoria> GetAll(int skip, int take)
    {
        int tamMaximoPagina = 50;
        take = take > tamMaximoPagina ? tamMaximoPagina : take;
        return _context.Categorias.AsNoTracking().Skip(skip).Take(take).ToList();
    }
    public Categoria Get(int id)
    {
        return _context.Categorias.Find(id);
    }
    public Categoria Create(Categoria categoria)
    {
        _context.Add(categoria);
        return categoria; 
    }
    public Categoria Update(Categoria categoria)
    {
        var categoriaExistente = _context.Categorias.Find(categoria.Id);
        if (categoriaExistente == null)
        {
            throw new KeyNotFoundException("Categoria não encontrada.");
        }
        _context.Entry(categoriaExistente).CurrentValues.SetValues(categoria);
        return categoria;
    }
    public Categoria Delete(int id)
    {
        var categoria = _context.Categorias.Find(id);
        _context.Categorias.Remove(categoria);
        return categoria;
    }
}
