using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Produto> GetAll(int skip, int take)
    {
        int tamMaximoPagina = 2;
        take = take > tamMaximoPagina ? tamMaximoPagina : take;
        return _context.Produtos.AsNoTracking().Skip(skip).Take(take).ToList();
    }
    public Produto Get(int id)
    {
        return _context.Produtos.Find(id);
    }
    public Produto Create(Produto produto)
    {
        _context.Produtos.Add(produto);
        return produto;
    }
    public bool Update(Produto produto)
    {
        if (_context.Produtos.Any(p => p.Id == produto.Id))
        {
            _context.Produtos.Update(produto);
            return true;
        }
        return false;
    }
    public bool Delete(Produto produto)
    {
        if (_context.Produtos.Any(p => p.Id == produto.Id))
        {
            _context.Produtos.Remove(produto);
            return true;
        }
        return false;
    }
}
