using APICatalogo.Models;

namespace APICatalogo.Repositories;

public interface ICategoriaRepository
{
    IEnumerable<Categoria> GetAll(int skip, int take);
    Categoria Get(int id);
    Categoria Create(Categoria categoria);  
    Categoria Update(Categoria categoria);
    Categoria Delete(int id);
}
