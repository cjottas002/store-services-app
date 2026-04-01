using AutoMapper;
using StoreServices.Api.Book.Model;

namespace StoreServices.Api.Book.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<LibraryMaterial, BookMaterialDto>();
    }
}
