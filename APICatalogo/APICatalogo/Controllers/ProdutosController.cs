﻿using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> Get()
    {
        try
        {
            var produtos = await _context.Produtos.AsNoTracking().ToListAsync();
            if (produtos is null)
                return NotFound("Produtos não encontrados...");
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Ocorreu um erro na requisição: {ex.Message}");
        }

    }

    [HttpGet("{id:int}", Name= "ObterProdutoPorId")]
    public async Task <ActionResult<Produto>> Get(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null) return NotFound("Produto não encontrado...");
        return produto;
    }
    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        _context.Produtos.Add(produto);
        _context.SaveChanges();
        return new CreatedAtRouteResult("ObterProdutoPorId",new {id = produto.Id}, produto);
    }
    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.Id)
            return BadRequest("O id informado deve ser igual ao id do produto a ser atualizado");
        _context.Entry(produto).State = EntityState.Modified;
        _context.SaveChanges();
        return Ok(produto);
    }
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var produto = _context.Produtos.Find(id);
        if (produto is null) return NotFound("Produto não encontrado...");

        _context.Produtos.Remove(produto);
        _context.SaveChanges();
        return Ok(produto);

    }

}
