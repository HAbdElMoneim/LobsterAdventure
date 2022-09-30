using LobsterAdventure.Caching.Services;
using LobsterAdventure.Core.Extentions;
using LobsterAdventure.Core.Models;

namespace LobsterAdventure.Api.Services
{
    public class AdventureService : IAdventureService
    {
        private readonly IAdventureCacheService _adventureCacheService;
        private readonly ILogger<AdventureService> _logger;

        /// <summary>
        /// AdventureTreeNodeService
        /// </summary>
        /// <param name="adventureCacheService"></param>
        /// <param name="logger"></param>
        public AdventureService(IAdventureCacheService adventureCacheService, ILogger<AdventureService> logger)
        {
            _adventureCacheService = adventureCacheService;
            _logger = logger;
        }

        /// <summary>
        /// AddNewAdventure
        /// </summary>
        /// <param name="adventureTreeArray"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddNewAdventure(string?[] adventureTreeArray, CancellationToken cancellationToken)
        {
            if (adventureTreeArray == null || adventureTreeArray.Length == 0 || adventureTreeArray[0] == null)
            {
                _logger.AdventureArrayIsNullOrEmpty();
                return false;
            }

            _logger.LogStepInfo("AddNewAdventure method :adventure Array", "adventureTreeArray", adventureTreeArray.ToJson(), string.Empty);
            var adventureNode = GetAdventureTreeFromArray(adventureTreeArray);
            _logger.LogStepInfo("AddNewAdventure method :Get Adventure Tree From Array", "adventureNode", adventureNode.ToJson(), string.Empty);

            return await _adventureCacheService.AddAdventure(adventureNode, cancellationToken);
        }

        /// <summary>
        /// GetAdventure
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>FullAdventureTree</returns>
        public async Task<AdventureTreeNode?> GetAdventure(CancellationToken cancellationToken)
        {
            var adventureNode = await _adventureCacheService.GetAdventure(cancellationToken);
            _logger.LogStepInfo("GetAdventure method : GetAdventure", "adventureNode", adventureNode.ToJson(), string.Empty);

            return adventureNode;
        }
        private AdventureTreeNode GetAdventureTreeFromArray(string?[] treeArray)
        {
            AdventureTreeNode GetFromArray(int index)
            {
                if (index >= treeArray.Length || treeArray[index] == null) return null;
                var node = new AdventureTreeNode(index, treeArray[index]);
                node.LeftChild = GetFromArray(index * 2 + 1);
                node.RightChild = GetFromArray(index * 2 + 2);

                return node;
            }

            return GetFromArray(0);
        }
    }
}
