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
    }
}
