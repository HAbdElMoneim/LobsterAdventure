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
    public class AdventureServiceTests
    {
        private readonly IAdventureService _adventureService;
        private readonly Mock<IAdventureCacheService> _adventureCacheServiceMock;
        private readonly Mock<ILogger<AdventureService>> _loggerMock;

        public AdventureServiceTests()
        {
            _adventureCacheServiceMock = new Mock<IAdventureCacheService>();
            _loggerMock = new Mock<ILogger<AdventureService>>();

            _adventureService = new AdventureService(_adventureCacheServiceMock.Object, _loggerMock.Object);
        }

        [Theory]
        [InlineAutoData(new object[] { new string[] { "A", "B", "C" } })]
        internal async Task GivenUseCase_WhenAddNewAdventureFromController_TrueIsReturned(string?[] adventureArray)
        {
            // Arrange
            _adventureCacheServiceMock.Setup(acs => acs.AddAdventure(It.IsAny<AdventureTreeNode>(), default)).ReturnsAsync(true);

            // Action
            var result = await _adventureService.AddNewAdventure(adventureArray, default);

            // Assert
            Assert.IsType<bool>(result);
            ((bool)result).Should().Be(true);

            _adventureCacheServiceMock.Verify(x => x.AddAdventure(It.Is<AdventureTreeNode>(WithExpectedNodes(adventureArray)), default), Times.Exactly(1));
        }

        [Fact]
        internal async Task GivenUseCase_WhenGetAdventureFromController_AdventureTreeIsReturned()
        {
            // Arrange
            var mockedResult = new AdventureTreeNode(0, "A", true, new AdventureTreeNode(1, "B"), new AdventureTreeNode(2, "C"));
            _adventureCacheServiceMock.Setup(acs => acs.GetAdventure(default)).ReturnsAsync(mockedResult);

            // Action
            var result = await _adventureService.GetAdventure(default);

            // Assert
            Assert.IsType<AdventureTreeNode>(result);
            ((AdventureTreeNode)result).NodeId.Should().Be(0);
            ((AdventureTreeNode)result).LeftChild.NodeId.Should().Be(1);
            ((AdventureTreeNode)result).RightChild.NodeId.Should().Be(2);

            _adventureCacheServiceMock.Verify(x => x.GetAdventure(default), Times.Exactly(1));
        }

        private static Expression<System.Func<AdventureTreeNode, bool>> WithExpectedNodes(string[] adventureArray)
        {
            return x => x.NodeText == adventureArray[x.NodeId] &&
                        x.LeftChild.NodeText == adventureArray[x.NodeId + 1] &&
                        x.RightChild.NodeText == adventureArray[x.NodeId + 2];
        }
    }
}
