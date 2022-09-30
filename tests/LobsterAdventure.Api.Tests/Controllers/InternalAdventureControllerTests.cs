using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using LobsterAdventure.Api.Services;
using LobsterAdventure.Controllers;
using LobsterAdventure.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LobsterAdventure.Api.Tests.Controllers
{
    public class InternalAdventureControllerTests
    {
        private readonly InternalAdventureController _internalController;
        private readonly Mock<IAdventureService> _adventureServiceMock;
        private readonly Mock<ILogger<InternalAdventureController>> _loggerMock;

        public InternalAdventureControllerTests()
        {
            _adventureServiceMock = new Mock<IAdventureService>();
            _loggerMock = new Mock<ILogger<InternalAdventureController>>();

            _internalController = new InternalAdventureController(_adventureServiceMock.Object, _loggerMock.Object);
        }

        [Theory]
        [InlineAutoData(new object[] { new string[] { "A", "B", "C" }})]
        internal async Task GivenUseCase_WhenCreateNewAdventureFromController_TrueIsReturned(string?[] adventureArray)
        {
            // Arrange
            _adventureServiceMock.Setup(asm => asm.AddNewAdventure(It.IsAny<string?[]>(), default)).ReturnsAsync(true);

            // Action
            var result = await _internalController.CreateNewAdventure(adventureArray);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            okObjectResult.Value.Should().BeEquivalentTo(true);
        }

        [Theory]
        [InlineAutoData(new object[] { new string[] {} })]
        internal async Task GivenUseCase_WhenCreateNewAdventureFromControllerWithInvalidInput_BadRequestReturned(string?[] adventureArray)
        {
            // Arrange
            _adventureServiceMock.Setup(asm => asm.AddNewAdventure(It.IsAny<string?[]>(), default)).ReturnsAsync(true);

            // Action
            var result = await _internalController.CreateNewAdventure(adventureArray);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _adventureServiceMock.Verify(x => x.AddNewAdventure(It.IsAny<string?[]>(), default), Times.Never);
        }
        
        [Theory]
        [InlineAutoData(new object[] { new string[] { "A", "B", "C" } })]
        internal async Task GivenUseCase_WhenCreateNewAdventureFromControllerWithUnExpectedException_BadRequestReturned(string?[] adventureArray)
        {
            // Arrange
            _adventureServiceMock.Setup(asm => asm.AddNewAdventure(It.IsAny<string?[]>(), default)).Throws(new System.Exception());

            // Action
            var result = await _internalController.CreateNewAdventure(adventureArray);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _adventureServiceMock.Verify(x => x.AddNewAdventure(It.IsAny<string?[]>(), default), Times.Exactly(1));
        }

        [Fact]
        internal async Task GivenUseCase_WhenGetAdventureFromController_AdventureIsReturned()
        {
            // Arrange
            _adventureServiceMock.Setup(asm => asm.GetAdventure(default)).Returns(Task.FromResult(new AdventureTreeNode(1,"test")));

            // Action
            var result = await _internalController.GetAdventure();

            // Assert
            Assert.IsType<ActionResult<AdventureTreeNode>>(result);
            var okObjectResult = (OkObjectResult)result.Result;
            okObjectResult.Should().NotBeNull();
            ((AdventureTreeNode)okObjectResult.Value).NodeId.Should().Be(1);
            ((AdventureTreeNode)okObjectResult.Value).NodeText.Should().Be("test");
        }

        [Fact]
        internal async Task GivenUseCase_WhenGetAdventureFromControllerWithNullReturnedValue_NotFoundReturned()
        {
            // Arrange
            _adventureServiceMock.Setup(asm => asm.GetAdventure(default));

            // Action
            var result = await _internalController.GetAdventure();

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            result.Value.Should().Be(null);
        }

        [Fact]
        internal async Task GivenUseCase_WhenGetAdventureFromControllerWithUnExpectedException_NotFoundReturned()
        {
            // Arrange
            _adventureServiceMock.Setup(asm => asm.GetAdventure(default)).Throws(new System.Exception());

            // Action
            var result = await _internalController.GetAdventure();

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            _adventureServiceMock.Verify(x => x.GetAdventure(default), Times.Exactly(1));
        }
    }
}
