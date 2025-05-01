using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Models.DTOs;
using APICatalogo.Repositories;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _iof;
    private readonly IMapper _mapper;
    public ProdutosController(IUnitOfWork iof, IMapper mapper)
    {
        _iof = iof;
        _mapper = mapper;
    }
    [HttpGet]
    public ActionResult<IEnumerable<ProdutoDTO>> Get()
    {
        try
        {
            var produtos = _iof.ProdutoRepository.GetAll();
            if (!produtos.Any())
                return NoContent();

            var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
            return Ok(produtosDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Ocorreu um erro na requisição: {ex.Message}");
        }
    }

    [HttpGet("{id:int}", Name= "ObterProdutoPorId")]
    public ActionResult<ProdutoDTO> Get(int id)
    {
        var produto = _iof.ProdutoRepository.Get(id);
        if (produto is null) return NotFound("Produto não encontrado...");
        
        var produtoDto = _mapper.Map<ProdutoDTO>(produto);
        return Ok(produtoDto);
    }
    [HttpPost]
    public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDto)
    {
        var produto = _mapper.Map<Produto>(produtoDto);
        var produtoCriado = _iof.ProdutoRepository.Create(produto);
        _iof.Commit();

        var novoProdutoDTO = _mapper.Map<ProdutoDTO>(produtoCriado);
        return new CreatedAtRouteResult("ObterProdutoPorId",new {id = novoProdutoDTO.Id}, novoProdutoDTO);
    }

    [HttpPatch("{id:int}/updatePartial")]
    public ActionResult<ProdutoDtoUpdateRequest> Patch (int id, JsonPatchDocument<ProdutoDtoUpdateRequest> prodRequestDTO)
    {
        if (prodRequestDTO is null || id <= 0)
            return BadRequest("Dados Inválidos");

        var produto = _iof.ProdutoRepository.Get(id);
        if (produto is null)
            return NotFound("Produto não encontrado");

        var produtoRquest = _mapper.Map<ProdutoDtoUpdateRequest>(produto);
        prodRequestDTO.ApplyTo(produtoRquest, ModelState);

        if (!TryValidateModel(produtoRquest))
            return BadRequest(ModelState);

        _mapper.Map(produtoRquest, produto);
        _iof.ProdutoRepository.Update(produto);
        _iof.Commit();

        return Ok(_mapper.Map<ProdutoDtoUpdateResponse>(produto));
    }

    [HttpPut("{id:int}")]
    public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.Id)
            return BadRequest("O id informado deve ser igual ao id do produto a ser atualizado");

        var produtoExistente = _iof.ProdutoRepository.Get(id); 
        if (produtoExistente is null)
            return NotFound("Produto não encontrado");

        _mapper.Map(produtoDto, produtoExistente);

        _iof.ProdutoRepository.Update(produtoExistente);
        _iof.Commit();
        return Ok(_mapper.Map<ProdutoDTO>(produtoExistente));
    }
    [HttpDelete("{id:int}")]
    public ActionResult<ProdutoDTO> Delete(int id)
    {
        var produto = _iof.ProdutoRepository.Get(id);
        if (produto is null) return NotFound("Produto não encontrado...");

        _iof.ProdutoRepository.Delete(produto);
        _iof.Commit();
        return Ok(_mapper.Map<ProdutoDTO>(produto));
    }
}
