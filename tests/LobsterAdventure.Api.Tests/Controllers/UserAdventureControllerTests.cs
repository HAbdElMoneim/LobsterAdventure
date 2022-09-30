using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using LobsterAdventure.Api.Services;
using LobsterAdventure.Controllers;
using LobsterAdventure.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LobsterAdventure.Api.Tests.Controllers
{
    public class UserAdventureControllerTests
    {
        private readonly UserAdventureController _userController;
        private readonly Mock<IUserAdventureService> _userAdventureServiceMock;
        private readonly Mock<ILogger<UserAdventureController>> _loggerMock;

        public UserAdventureControllerTests()
        {
            _userAdventureServiceMock = new Mock<IUserAdventureService>();
            _loggerMock = new Mock<ILogger<UserAdventureController>>();

            _userController = new UserAdventureController(_userAdventureServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        internal async Task GivenUseCase_WhenStartNewAdventureFromController_AdventureFirstStepReturned()
        {
            // Arrange
            var mockedResult = new AdventureTreeNode(0, "A", true, new AdventureTreeNode(1, "B"), new AdventureTreeNode(2, "C"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "123") })));
           
            _userController.ControllerContext.HttpContext = httpContextMock.Object;
            _userAdventureServiceMock.Setup(asm => asm.CreateUserAdventure(It.IsAny<string>(), default)).ReturnsAsync(mockedResult);

            // Action
            var result = await _userController.StartNewAdventure();

            // Assert
            Assert.IsType<ActionResult<AdventureTreeNode>>(result);
            var okObjectResult = (OkObjectResult)result.Result;
            okObjectResult.Should().NotBeNull();
            ((AdventureTreeNode)okObjectResult.Value).NodeId.Should().Be(0);
            ((AdventureTreeNode)okObjectResult.Value).NodeText.Should().Be("A");
        }

        [Fact]
        internal async Task GivenUseCase_WhenStartNewAdventureWithNoDataReturnedFromController_NotFoundReturned()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "123") })));
           
            _userController.ControllerContext.HttpContext = httpContextMock.Object;
            _userAdventureServiceMock.Setup(asm => asm.CreateUserAdventure(It.IsAny<string>(), default));

            // Action
            var result = await _userController.StartNewAdventure();

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            _userAdventureServiceMock.Verify(x => x.CreateUserAdventure(It.IsAny<string>(), default), Times.Exactly(1));
        }

        [Fact]
        internal async Task GivenUseCase_WhenStartNewAdventureWithExceptionThrownFromController_NotFoundReturned()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "123") })));
           
            _userController.ControllerContext.HttpContext = httpContextMock.Object;
            _userAdventureServiceMock.Setup(asm => asm.CreateUserAdventure(It.IsAny<string>(), default)).Throws(new System.Exception());

            // Action
            var result = await _userController.StartNewAdventure();

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            _userAdventureServiceMock.Verify(x => x.CreateUserAdventure(It.IsAny<string>(), default), Times.Exactly(1));
        }

        [Fact]
        internal async Task GivenUseCase_WhenStartNewAdventureWithInvalidUserFromController_UnauthorizedIsReturned()
        {
            // Arrange
            var mockedResult = new AdventureTreeNode(0, "A", true, new AdventureTreeNode(1, "B"), new AdventureTreeNode(2, "C"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", string.Empty) })));
           
            _userController.ControllerContext.HttpContext = httpContextMock.Object;
            _userAdventureServiceMock.Setup(asm => asm.CreateUserAdventure(It.IsAny<string>(), default)).ReturnsAsync(mockedResult);

            // Action
            var result = await _userController.StartNewAdventure();

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
            _userAdventureServiceMock.Verify(x => x.CreateUserAdventure(It.IsAny<string>(), default), Times.Never);
        }

        [Theory]
        [InlineAutoData(1)]
        internal async Task GivenUseCase_WhenGetAdventureNextStepFromController_AdventureNextStepReturned(int nodeId)
        {
            // Arrange
            var mockedResult = new AdventureTreeNode(nodeId, "B", true, new AdventureTreeNode(3, "C"), new AdventureTreeNode(4, "D"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "123") })));

            _userController.ControllerContext.HttpContext = httpContextMock.Object;
            _userAdventureServiceMock.Setup(asm => asm.ProcessUserAdventure(nodeId, It.IsAny<string>(), default)).ReturnsAsync(mockedResult);

            // Action
            var result = await _userController.GetAdventureNextStep(nodeId);

            // Assert
            Assert.IsType<ActionResult<AdventureTreeNode>>(result);
            var okObjectResult = (OkObjectResult)result.Result;
            okObjectResult.Should().NotBeNull();
            ((AdventureTreeNode)okObjectResult.Value).NodeId.Should().Be(nodeId);
            ((AdventureTreeNode)okObjectResult.Value).NodeText.Should().Be("B");
            ((AdventureTreeNode)okObjectResult.Value).LeftChild.NodeId.Should().Be(3);
            ((AdventureTreeNode)okObjectResult.Value).RightChild.NodeId.Should().Be(4);
        }

        [Theory]
        [InlineAutoData(0)]
        internal async Task GivenUseCase_WhenGetAdventureNextStepWithInvalidNodeIdFromController_AdventureNextStepReturned(int nodeId)
        {
            // Arrange
            var mockedResult = new AdventureTreeNode(nodeId, "B", true, new AdventureTreeNode(3, "C"), new AdventureTreeNode(4, "D"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "123") })));

            _userController.ControllerContext.HttpContext = httpContextMock.Object;
            _userAdventureServiceMock.Setup(asm => asm.ProcessUserAdventure(nodeId, It.IsAny<string>(), default)).ReturnsAsync(mockedResult);

            // Action
            var result = await _userController.GetAdventureNextStep(nodeId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            _userAdventureServiceMock.Verify(x => x.ProcessUserAdventure(nodeId, It.IsAny<string>(), default), Times.Never);
        }

        [Fact]
        internal async Task GivenUseCase_WhenGetAdventureResultFromController_AdventureResultReturned()
        {
            // Arrange
            var mockedResult = new AdventureTreeNode(1, "B", true, new AdventureTreeNode(3, "C"), new AdventureTreeNode(4, "D"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "123") })));

            _userController.ControllerContext.HttpContext = httpContextMock.Object;
            _userAdventureServiceMock.Setup(asm => asm.GetUserAdventureResult(It.IsAny<string>(), default)).ReturnsAsync(mockedResult);

            // Action
            var result = await _userController.GetAdventureResult();

            // Assert
            Assert.IsType<ActionResult<AdventureTreeNode>>(result);
            var okObjectResult = (OkObjectResult)result.Result;
            okObjectResult.Should().NotBeNull();
            ((AdventureTreeNode)okObjectResult.Value).NodeId.Should().Be(1);
            ((AdventureTreeNode)okObjectResult.Value).NodeText.Should().Be("B");
            ((AdventureTreeNode)okObjectResult.Value).LeftChild.NodeId.Should().Be(3);
            ((AdventureTreeNode)okObjectResult.Value).RightChild.NodeId.Should().Be(4);
        }
    }
}
