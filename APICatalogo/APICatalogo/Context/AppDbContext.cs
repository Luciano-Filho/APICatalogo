﻿using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace APICatalogo.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get;set; }
}
