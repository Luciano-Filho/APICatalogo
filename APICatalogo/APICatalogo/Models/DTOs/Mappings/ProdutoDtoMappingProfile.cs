﻿using AutoMapper;

namespace APICatalogo.Models.DTOs.Mappings;

public class ProdutoDtoMappingProfile : Profile
{
    public ProdutoDtoMappingProfile()
    {
        CreateMap<Produto, ProdutoDTO>().ReverseMap();
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<ProdutoDtoUpdateRequest, Produto>().ReverseMap();
        CreateMap<ProdutoDtoUpdateResponse, Produto>().ReverseMap();
    }
}
