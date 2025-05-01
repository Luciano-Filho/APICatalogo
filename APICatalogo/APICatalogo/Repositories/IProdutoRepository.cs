using APICatalogo.Models;

namespace APICatalogo.Repositories;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> GetAllAsync(int skip, int take);
    Task<Produto> GetAsync(int id);
    Produto Create(Produto produto);
    bool Delete(Produto produto);
    bool Update(Produto produto);
}
