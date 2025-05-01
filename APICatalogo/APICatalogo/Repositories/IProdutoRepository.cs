using APICatalogo.Models;

namespace APICatalogo.Repositories;

public interface IProdutoRepository
{
    IEnumerable<Produto> GetAll(int skip, int take);
    Produto Get(int id);
    Produto Create(Produto produto);
    bool Delete(Produto produto);
    bool Update(Produto produto);
}
