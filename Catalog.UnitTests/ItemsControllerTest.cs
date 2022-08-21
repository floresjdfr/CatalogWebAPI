using Catalog.Api.Controllers;
using Catalog.Api.DTOs;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.UnitTests;

public class ItemsControllerTest
{
    private readonly Mock<IItemsRepository> _repositoryStub = new();
    private readonly Mock<ILogger<ItemsController>> _loggerStub = new();
    private readonly Random _rand = new();

    [Fact]
    //Naming convension: UnitOfWork_StateUnderTest_ExpectedBehavior
    public async Task GetItemAsync_WithUnexistingItem_NotFound()
    {
        //Arange
        _repositoryStub
            .Setup(repo => repo.GetItemByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Item?)null);

        var controller = new ItemsController(_repositoryStub.Object, _loggerStub.Object);

        //Act
        var result = await controller.GetItemByIdAsync(Guid.NewGuid());

        //Assert
        //Assert.IsType<NotFoundResult>(result.Result); => This way is using Assert

        result.Result.Should().BeOfType<NotFoundResult>(); //=> This way is using the library FluentAssertion
    }

    [Fact]
    public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
    {
        //Arange

        var expectedItem = CreateRandomItem();
        _repositoryStub
            .Setup(repo => repo.GetItemByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedItem);

        var controller = new ItemsController(_repositoryStub.Object, _loggerStub.Object);

        //Act
        var result = await controller.GetItemByIdAsync(Guid.NewGuid());

        //Assert
        result.Value
            .Should()
            .BeEquivalentTo(expectedItem, options => options.ComparingByMembers<Item>());
    }

    [Fact]
    public async Task GetItemAsync_WithExistingItems_ReturnsAllItems()
    {
        //Arange
        var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
        _repositoryStub.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(expectedItems);
        var controller = new ItemsController(_repositoryStub.Object, _loggerStub.Object);

        //Act
        var actualItems = await controller.GetItemsAsync();

        //Assert
        actualItems
            .Should()
            .BeEquivalentTo(expectedItems, options => options.ComparingByMembers<Item>());
    }

    [Fact]
    public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
    {
        //Arange
        var itemToCreate = new CreateItemDto(Guid.NewGuid().ToString(), _rand.Next(1000));

        var controller = new ItemsController(_repositoryStub.Object, _loggerStub.Object);

        //Act
        var result = await controller.CreateItemAsync(itemToCreate);

        //Assert
        var createdItem = (result.Result as CreatedAtActionResult)!.Value as ItemDto;

        itemToCreate
            .Should()
            .BeEquivalentTo(
                createdItem,
                options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );

        createdItem!.Id.Should().NotBeEmpty();
        createdItem.CreatedDate
            .Should()
            .BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(1000));
    }

    [Fact]
    public async Task UpdateItemAsync_WithItemToUpdate_ReturnsNoContent()
    {
        //Arange

        var expectedItem = CreateRandomItem();
        _repositoryStub
            .Setup(repo => repo.GetItemByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedItem);

        var itemId = expectedItem.Id;
        var itemToUpdate = new UpdateItemDto(Guid.NewGuid().ToString(), expectedItem.Price + 3);

        var controller = new ItemsController(_repositoryStub.Object, _loggerStub.Object);

        //Act
        var result = await controller.UpdateItem(itemId, itemToUpdate);

        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteItemAsync_WithItemToDelete_ReturnsNoContent()
    {
        //Arange

        var expectedItem = CreateRandomItem();
        _repositoryStub
            .Setup(repo => repo.GetItemByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedItem);

        var itemId = expectedItem.Id;

        var controller = new ItemsController(_repositoryStub.Object, _loggerStub.Object);

        //Act
        var result = await controller.DeleteItemAsync(itemId);

        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    private Item CreateRandomItem()
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Price = _rand.Next(1000),
            CreatedDate = DateTimeOffset.UtcNow
        };
    }
}
