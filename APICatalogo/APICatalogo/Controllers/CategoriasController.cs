using System.Text;
using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Models.DTOs;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _iof;
    //string caminhoLog = "C:\\Users\\Luciano\\Documents\\APIs\\Desenvolvimento\\Logs";
    private readonly IMapper _mapper;
    public CategoriasController(IUnitOfWork iof, IMapper mapper)
    {
        _iof = iof;
        _mapper = mapper;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<CategoriaDTO>> GetAll(int skip, int take)
    {
        var categorias = _iof.CategoriaRepository.GetAll(skip, take);
        if (!categorias.Any())
            return NotFound();

        var categoriasDTO = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

        return Ok(categoriasDTO);
    }

    [HttpGet("{id:int}", Name = "CategoriaPorId")]
    public ActionResult<CategoriaDTO> Get(int id)
    {
        var categoria = _iof.CategoriaRepository.Get(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada...");

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return Ok(categoriaDTO);
    }
    [HttpPost]
    public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDto)
    {
        var categoria = _mapper.Map<Categoria>(categoriaDto);
        var categoriaCriada = _iof.CategoriaRepository.Create(categoria);
        _iof.Commit();
        var novaCategoriaDTO = _mapper.Map<CategoriaDTO>(categoriaCriada);
        return new CreatedAtRouteResult("CategoriaPorId", new { id = novaCategoriaDTO.Id }, novaCategoriaDTO);
    }
    [HttpPut("{id:int}")]
    public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.Id)
            return BadRequest("O id informado deve ser o mesmo id da categoria");

        var categoria = _iof.CategoriaRepository.Get(id);
        if (categoria is null)
            return NotFound("Não há categoria para o id informado");

        _mapper.Map(categoriaDto, categoria);
        var categoriaAtualiza = _iof.CategoriaRepository.Update(categoria);
        _iof.Commit();

        var categoriaDtoAtualizada = _mapper.Map<CategoriaDTO>(categoriaAtualiza);

        return Ok(categoriaDtoAtualizada);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _iof.CategoriaRepository.Get(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada...");
        _iof.CategoriaRepository.Delete(id);
        _iof.Commit();
        return Ok(categoria);
    }

}
