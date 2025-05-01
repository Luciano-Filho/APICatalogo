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

    public async Task<IEnumerable<Produto>> GetAllAsync(int skip = 0, int take = 10)
    {
        int tamMaximoPagina = 50;
        take = take > tamMaximoPagina ? tamMaximoPagina : take;
        return await _context.Produtos.AsNoTracking().Skip(skip).Take(take).ToListAsync();
    }
    public async Task<Produto> GetAsync(int id)
    {
        return await _context.Produtos.FindAsync(id);
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
