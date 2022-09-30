using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using LobsterAdventure.Api.Services;
using LobsterAdventure.Caching.Services;
using LobsterAdventure.Core.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LobsterAdventure.Api.Tests.Services
{
    public class UserAdventureServiceTests
    {
        private readonly IUserAdventureService _userAdventureService;
        private readonly Mock<IAdventureCacheService> _adventureCacheServiceMock;
        private readonly Mock<ILogger<UserAdventureService>> _loggerMock;

        public UserAdventureServiceTests()
        {
            _adventureCacheServiceMock = new Mock<IAdventureCacheService>();
            _loggerMock = new Mock<ILogger<UserAdventureService>>();

            _userAdventureService = new UserAdventureService(_adventureCacheServiceMock.Object, _loggerMock.Object);
        }

        [Theory]
        [InlineAutoData("123")]
        internal async Task GivenUseCase_WhenCreateUserAdventureFromController_FirstAdventureNodeIsReturned(string userId)
        {
            // Arrange
            var mockedResult = new AdventureTreeNode(0, "A", true, new AdventureTreeNode(1, "B"), new AdventureTreeNode(2, "C"));
            _adventureCacheServiceMock.Setup(acs => acs.GetAdventure(default)).ReturnsAsync(mockedResult);
            _adventureCacheServiceMock.Setup(acs => acs.UpdateUserAdventure(userId, It.IsAny<AdventureTreeNode>(), default)).ReturnsAsync(true);

            // Action
            var result = await _userAdventureService.CreateUserAdventure(userId, default);

            // Assert
            Assert.IsType<AdventureTreeNode>(result);
            ((AdventureTreeNode)result).NodeId.Should().Be(0);
            ((AdventureTreeNode)result).LeftChild.NodeId.Should().Be(1);
            ((AdventureTreeNode)result).RightChild.NodeId.Should().Be(2);

            _adventureCacheServiceMock.Verify(x => x.GetAdventure(default), Times.Exactly(1));
            _adventureCacheServiceMock.Verify(x => x.UpdateUserAdventure(userId, It.Is<AdventureTreeNode>(WithExpectedValues(mockedResult)), default), Times.Exactly(1));
        }
        
        [Theory]
        [InlineAutoData("123", 1)]
        internal async Task GivenUseCase_WhenProcessUserAdventureFromController_NextAdventureNodeIsReturned(string userId, int nodeId)
        {
            // Arrange
            var mockedResult = new AdventureTreeNode(nodeId, "B", true, new AdventureTreeNode(3, "C"), new AdventureTreeNode(4, "D"));
            _adventureCacheServiceMock.Setup(acs => acs.GetUserAdventure(userId, default)).ReturnsAsync(mockedResult);
            _adventureCacheServiceMock.Setup(acs => acs.UpdateUserAdventure(userId, It.IsAny<AdventureTreeNode>(), default)).ReturnsAsync(true);

            // Action
            var result = await _userAdventureService.ProcessUserAdventure(nodeId, userId, default);

            // Assert
            Assert.IsType<AdventureTreeNode>(result);
            ((AdventureTreeNode)result).NodeId.Should().Be(nodeId);
            ((AdventureTreeNode)result).LeftChild.NodeId.Should().Be(3);
            ((AdventureTreeNode)result).RightChild.NodeId.Should().Be(4);

            _adventureCacheServiceMock.Verify(x => x.GetUserAdventure(userId, default), Times.Exactly(1));
            _adventureCacheServiceMock.Verify(x => x.UpdateUserAdventure(userId, It.Is<AdventureTreeNode>(WithExpectedValues(mockedResult)), default), Times.Exactly(1));
        }

        [Theory]
        [InlineAutoData("123")]
        internal async Task GivenUseCase_WhenProcessGetUserAdventureResultFromController_UserAdventureResultIsReturned(string userId)
        {
            // Arrange
            var mockedResult = new AdventureTreeNode(0, "A", true,
                new AdventureTreeNode(1, "B", true, null, null),
                new AdventureTreeNode(2, "C", false, new AdventureTreeNode(5, "F"), new AdventureTreeNode(6, "G")));

            _adventureCacheServiceMock.Setup(acs => acs.GetUserAdventure(userId, default)).ReturnsAsync(mockedResult);
            _adventureCacheServiceMock.Setup(acs => acs.UpdateUserAdventure(userId, It.IsAny<AdventureTreeNode>(), default)).ReturnsAsync(true);

            // Action
            var result = await _userAdventureService.GetUserAdventureResult(userId, default);

            // Assert
            Assert.IsType<AdventureTreeNode>(result);
            ((AdventureTreeNode)result).NodeId.Should().Be(0);
            ((AdventureTreeNode)result).IsSelected.Should().Be(true);
            ((AdventureTreeNode)result).LeftChild.NodeId.Should().Be(1);
            ((AdventureTreeNode)result).LeftChild.IsSelected.Should().Be(true);
            ((AdventureTreeNode)result).RightChild.NodeId.Should().Be(2);
            ((AdventureTreeNode)result).RightChild.IsSelected.Should().Be(false);
            ((AdventureTreeNode)result).RightChild.LeftChild.Should().Be(null);
            ((AdventureTreeNode)result).RightChild.RightChild.Should().Be(null);

            _adventureCacheServiceMock.Verify(x => x.GetUserAdventure(userId, default), Times.Exactly(1));
            _adventureCacheServiceMock.Verify(x => x.UpdateUserAdventure(userId, It.IsAny<AdventureTreeNode>(), default), Times.Never);
        }

        private static Expression<Func<AdventureTreeNode, bool>> WithExpectedValues(AdventureTreeNode mockedResult)
        {
            return x => x.NodeId == mockedResult.NodeId &&
                        x.NodeText == mockedResult.NodeText &&
                        x.IsSelected == true &&
                        x.LeftChild.NodeId == mockedResult.LeftChild.NodeId &&
                        x.RightChild.NodeId == mockedResult.RightChild.NodeId;
        }
    }
}
