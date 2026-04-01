using AutoMapper;
using StoreServices.Api.Author.Model;

namespace StoreServices.Api.Author.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AuthorBook, AuthorDto>();
    }
}
