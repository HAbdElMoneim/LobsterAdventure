using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using LobsterAdventure.Caching.Enums;
using LobsterAdventure.Caching.Services;
using LobsterAdventure.Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LobsterAdventure.Caching.Tests.Services
{
    public class UserAdventureServiceTests
    {
        private readonly IAdventureCacheService _adventureCacheService;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly Mock<ILogger<AdventureCacheService>> _loggerMock;

        public UserAdventureServiceTests()
        {
            _distributedCacheMock = new Mock<IDistributedCache>();
            _loggerMock = new Mock<ILogger<AdventureCacheService>>();

            _adventureCacheService = new AdventureCacheService(_distributedCacheMock.Object, _loggerMock.Object);
        }

        //[Fact]
        //internal async Task GivenUseCase_WhenGetAdventureFromController_CachedAdventureTreeIsReturned()
        //{
        //    // Arrange
        //    var mockedResult = new AdventureTreeNode(0, "A", true, new AdventureTreeNode(1, "B"), new AdventureTreeNode(2, "C"));
        //    _distributedCacheMock.Setup(x => x.GetStringAsync(CacheKeysEnum.AdventureArray.ToString(), default)).ReturnsAsync(JsonSerializer.Serialize(mockedResult));

        //    // Action
        //    var result = await _adventureCacheService.GetAdventure(default);

        //    // Assert
        //    Assert.IsType<AdventureTreeNode>(result);
        //    ((AdventureTreeNode)result).NodeId.Should().Be(0);
        //    ((AdventureTreeNode)result).LeftChild.NodeId.Should().Be(1);
        //    ((AdventureTreeNode)result).RightChild.NodeId.Should().Be(2);

        //    _distributedCacheMock.Verify(x => x.GetStringAsync(CacheKeysEnum.AdventureArray.ToString(), default), Times.Exactly(1));
        //}
    }
}
