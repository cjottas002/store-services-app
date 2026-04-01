using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using StoreServices.Api.Book.Application;
using Xunit;

namespace StoreServices.Api.Book.Tests.Application
{
    public class MappingProfileTests
    {
        [Fact]
        public void MappingProfile_ConfigurationIsValid()
        {
            // Arrange
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, NullLoggerFactory.Instance);

            // Act & Assert
            config.AssertConfigurationIsValid();
        }
    }
}
