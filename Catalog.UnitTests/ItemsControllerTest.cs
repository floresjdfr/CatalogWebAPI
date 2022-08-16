using Catalog.Api.Controllers;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.UnitTests;

public class ItemsControllerTest
{
    [Fact]
    //Naming convension: UnitOfWork_StateUnderTest_ExpectedBehavior
    public async Task GetItemAsync_WithUnexistingItem_NotFound()
    {
        //Arange
        var repositoryStub = new Mock<IItemsRepository>();
        repositoryStub
            .Setup(repo => repo.GetItemByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Item?)null);

        var loggerStub = new Mock<ILogger<ItemsController>>();     
        var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);       

        //Act
        var result = await controller.GetItemByIdAsync(Guid.NewGuid());

        //Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
}
