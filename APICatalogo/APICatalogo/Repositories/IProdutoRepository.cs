using APICatalogo.Models;

namespace APICatalogo.Repositories;

public interface IProdutoRepository
{
    IEnumerable<Produto> GetAll();
    Produto Get(int id);
    Produto Create(Produto produto);
    bool Delete(Produto produto);
    bool Update(Produto produto);
}
