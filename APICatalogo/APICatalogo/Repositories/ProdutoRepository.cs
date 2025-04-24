using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Produto> GetAll()
    {
        return _context.Produtos.ToList();
    }
    public Produto Get(int id)
    {
        return _context.Produtos.Find(id);
    }
    public Produto Create(Produto produto)
    {
        _context.Produtos.Add(produto);
        _context.SaveChanges();
        return produto;
    }
    public bool Update(Produto produto)
    {
        if (_context.Produtos.Any(p => p.Id == produto.Id))
        {
            _context.Produtos.Update(produto);
            _context.SaveChanges();
            return true;
        }
        return false;
    }
    public bool Delete(Produto produto)
    {
        if (_context.Produtos.Any(p => p.Id == produto.Id))
        {
            _context.Produtos.Remove(produto);
            _context.SaveChanges();
            return true;
        }
        return false;
    }
}
