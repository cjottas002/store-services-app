using AutoMapper;
using StoreServices.Api.Book.Application;
using StoreServices.Api.Book.Model;

namespace StoreServices.Api.Book.Tests.Helpers
{
    public class TestMappingProfile : Profile
    {
        public TestMappingProfile()
        {
            CreateMap<LibraryMaterial, BookMaterialDto>();
        }
    }
}
