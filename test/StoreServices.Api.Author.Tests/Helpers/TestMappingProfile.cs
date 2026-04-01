using AutoMapper;
using StoreServices.Api.Author.Application;
using StoreServices.Api.Author.Model;

namespace StoreServices.Api.Author.Tests.Helpers
{
    public class TestMappingProfile : Profile
    {
        public TestMappingProfile()
        {
            CreateMap<AuthorBook, AuthorDto>();
        }
    }
}
