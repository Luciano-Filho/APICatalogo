using System.Text;
using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Models.DTOs;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("[Controller]")]
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
    [Authorize(Policy ="UserOnly")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAllAsync(int skip = 0, int take = 10)
    {
        var categorias = await _iof.CategoriaRepository.GetAllAsync(skip, take);
        if (!categorias.Any())
            return NotFound();

        var categoriasDTO = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

        return Ok(categoriasDTO);
    }

    [Authorize(Policy="AdminOnly")]
    [HttpGet("{id:int}", Name = "CategoriaPorId")]
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {
        var categoria = await _iof.CategoriaRepository.GetAsync(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada...");

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return Ok(categoriaDTO);
    }
    [HttpPost]
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
    {
        var categoria = _mapper.Map<Categoria>(categoriaDto);
        var categoriaCriada = _iof.CategoriaRepository.Create(categoria);
        await _iof.CommitAsync();
        var novaCategoriaDTO = _mapper.Map<CategoriaDTO>(categoriaCriada);
        return new CreatedAtRouteResult("CategoriaPorId", new { id = novaCategoriaDTO.Id }, novaCategoriaDTO);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.Id)
            return BadRequest("O id informado deve ser o mesmo id da categoria");

        var categoria = await _iof.CategoriaRepository.GetAsync(id);
        if (categoria is null)
            return NotFound("Não há categoria para o id informado");

        _mapper.Map(categoriaDto, categoria);
        var categoriaAtualiza = _iof.CategoriaRepository.Update(categoria);
        await _iof.CommitAsync();

        var categoriaDtoAtualizada = _mapper.Map<CategoriaDTO>(categoriaAtualiza);

        return Ok(categoriaDtoAtualizada);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var categoria = await _iof.CategoriaRepository.GetAsync(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada...");
        _iof.CategoriaRepository.Delete(id);
        await _iof.CommitAsync();
        return Ok(categoria);
    }
}
