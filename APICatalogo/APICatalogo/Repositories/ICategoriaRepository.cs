using APICatalogo.Models;

namespace APICatalogo.Repositories;

public interface ICategoriaRepository
{
    Task<IEnumerable<Categoria>> GetAllAsync(int skip, int take);
    Task<Categoria> GetAsync(int id);
    Categoria Create(Categoria categoria);  
    Categoria Update(Categoria categoria);
    Categoria Delete(int id);
}
