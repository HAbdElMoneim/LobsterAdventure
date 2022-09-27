using LobsterAdventure.Caching;
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
        /// AddNewAdventureTreeNode
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

            var adventureNode = GetAdventureTreeFromArray(adventureTreeArray);
            _logger.LogStepInfo("AddNewAdventure method :adventure Array", "adventureTreeArray", adventureTreeArray.ToJson(), string.Empty);
            _logger.LogStepInfo("AddNewAdventure method :Get Adventure Tree From Array", "adventureNode", adventureNode.ToJson(), string.Empty);

            return await _adventureCacheService.AddAdventure(adventureNode, cancellationToken);
        }

        /// <summary>
        /// GetUserAdventureResult
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>AdventureTreeNode?</returns>
        public async Task<AdventureTreeNode?> GetUserAdventureResult(string userId)
        {
            var userCachedAdventure = await _adventureCacheService.GetUserAdventure(userId);
            _logger.LogStepInfo("GetUserAdventureResult method : Getting user cached adventure", "UserCachedAdventure", userCachedAdventure.ToJson(), userId);

            if (userCachedAdventure == null)
            {
                _logger.UserAdventureIsNotAvailable(userId);
                return null;
            }

            if (!userCachedAdventure.IsSelected)
                return userCachedAdventure;

            ExtractSelectedSteps(userCachedAdventure);

            _logger.LogStepInfo("GetUserAdventureResult method : extract selected steps", "UserCachedAdventure", userCachedAdventure.ToJson(), userId);

            return userCachedAdventure;
        }

        /// <summary>
        /// ProcessUserAdventure
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>AdventureTreeNode</returns>
        public async Task<AdventureTreeNode?> ProcessUserAdventure(int nodeId, string userId, CancellationToken cancellationToken)
        {
            var userCachedAdventure = await _adventureCacheService.GetUserAdventure(userId) ?? await _adventureCacheService.GetAdventure();
            _logger.LogStepInfo("ProcessUserAdventure method : Getting user cached adventure", "UserCachedAdventure", userCachedAdventure.ToJson(), userId);

            if (userCachedAdventure == null)
            {
                _logger.UnableToFindAnyCachedAdventure();
                return null;
            }

            var nextNodes = LookupForChildNodesUsingBFS(userCachedAdventure, nodeId);
            _logger.LogStepInfo("ProcessUserAdventure method : Getting next adventure steps", "NextNodes", nextNodes.ToJson(), userId);

            if (nextNodes == null)
            {
                _logger.UserSelectedNodeIsNull(nodeId, userId);
                return null;
            }

            var updatedSuccessfully = await UpdateUserCachedAdventure(userId, userCachedAdventure, cancellationToken);

            _logger.LogStepInfo("ProcessUserAdventure method : Update user cached Adventure", "UpdateUserCachedAdventure()", userCachedAdventure.ToJson(), userId);


            if (!updatedSuccessfully)
            {
                _logger.UnableToUpdateUserCachedAdventure(userId);
                return null;
            }

            return nextNodes;
        }

        private void ExtractSelectedSteps(AdventureTreeNode userCachedAdventure)
        {
            var visited = new HashSet<int>();
            var nodeQueue = new Queue<AdventureTreeNode>();

            nodeQueue.Enqueue(userCachedAdventure);

            while (nodeQueue.Count > 0)
            {
                var qNode = nodeQueue.Dequeue();
                visited.Add(qNode.NodeId);


                if (!qNode.IsSelected)
                {
                    qNode.LeftChild = null;
                    qNode.RightChild = null;
                };

                if (qNode.LeftChild != null && !visited.Contains(qNode.LeftChild.NodeId))
                {
                    nodeQueue.Enqueue(qNode.LeftChild);

                }

                if (qNode.RightChild != null && !visited.Contains(qNode.RightChild.NodeId))
                {
                    nodeQueue.Enqueue(qNode.RightChild);

                }
            }
        }

        private async Task<bool> UpdateUserCachedAdventure(string userId, AdventureTreeNode cachedUserAdventure, CancellationToken cancellationToken)
        {
            return await _adventureCacheService.UpdateUserAdventure(userId, cachedUserAdventure, cancellationToken);
        }

        private AdventureTreeNode? LookupForChildNodesUsingBFS(AdventureTreeNode cachedUserAdventure, int nodeId)
        {
            var visited = new HashSet<int>();
            var nodeQueue = new Queue<AdventureTreeNode>();
            nodeQueue.Enqueue(cachedUserAdventure);

            while (nodeQueue.Count > 0)
            {
                var qNode = nodeQueue.Dequeue();
                visited.Add(qNode.NodeId);

                if (qNode.NodeId == nodeId)
                {
                    qNode.IsSelected = true;
                    return qNode;
                }

                if (qNode.LeftChild != null && !visited.Contains(qNode.LeftChild.NodeId))
                    nodeQueue.Enqueue(qNode.LeftChild);

                if (qNode.RightChild != null && !visited.Contains(qNode.RightChild.NodeId))
                    nodeQueue.Enqueue(qNode.RightChild);
            }

            return null;
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
