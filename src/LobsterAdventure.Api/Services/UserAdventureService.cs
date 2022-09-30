using LobsterAdventure.Caching.Services;
using LobsterAdventure.Core.Extentions;
using LobsterAdventure.Core.Models;

namespace LobsterAdventure.Api.Services
{
    public class UserAdventureService : IUserAdventureService
    {
        private readonly IAdventureCacheService _adventureCacheService;
        private readonly ILogger<UserAdventureService> _logger;

        /// <summary>
        /// AdventureTreeNodeService
        /// </summary>
        /// <param name="adventureCacheService"></param>
        /// <param name="logger"></param>
        public UserAdventureService(IAdventureCacheService adventureCacheService, ILogger<UserAdventureService> logger)
        {
            _adventureCacheService = adventureCacheService;
            _logger = logger;
        }

        /// <summary>
        /// CreateUserAdventure
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>First adventure step.</returns>
        public async Task<AdventureTreeNode?> CreateUserAdventure(string userId, CancellationToken cancellationToken)
        {
            var cachedAdventure = await _adventureCacheService.GetAdventure(cancellationToken);
            _logger.LogStepInfo("CreateUserAdventure method : Getting cached adventure", "UserCachedAdventure", cachedAdventure.ToJson(), userId);

            if (cachedAdventure == null)
            {
                _logger.UnableToFindAnyCreatedAdventure();
                return null;
            }

            var nextNodes = GetFirstChildNodes(cachedAdventure);
            _logger.LogStepInfo("ProcessUserAdventure method : Getting next adventure steps", "NextNodes", nextNodes.ToJson(), userId);

            if (nextNodes == null)
            {
                _logger.AdventureRootNodeNotFound();
                return null;
            }

            var updatedSuccessfully = await _adventureCacheService.UpdateUserAdventure(userId, cachedAdventure, cancellationToken);

            _logger.LogStepInfo("ProcessUserAdventure method : Update user cached Adventure", "UpdateUserCachedAdventure()", cachedAdventure.ToJson(), userId);


            if (!updatedSuccessfully)
            {
                _logger.UnableToUpdateUserCachedAdventure(userId);
                return null;
            }

            return nextNodes;
        }

        /// <summary>
        /// ProcessUserAdventure
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Next adventure step.</returns>
        public async Task<AdventureTreeNode?> ProcessUserAdventure(int nodeId, string userId, CancellationToken cancellationToken)
        {
            var userCachedAdventure = await _adventureCacheService.GetUserAdventure(userId, cancellationToken);
            _logger.LogStepInfo("ProcessUserAdventure method : Getting user cached adventure", "UserCachedAdventure", userCachedAdventure.ToJson(), userId);

            if (userCachedAdventure == null)
            {
                _logger.UnableToFindAnyCreatedAdventure();
                return null;
            }

            var nextNodes = LookupForChildNodesUsingBFS(userCachedAdventure, nodeId);
            _logger.LogStepInfo("ProcessUserAdventure method : Getting next adventure steps", "NextNodes", nextNodes.ToJson(), userId);

            if (nextNodes == null)
            {
                _logger.UserSelectedNodeIsNull(nodeId, userId);
                return null;
            }

            var updatedSuccessfully = await _adventureCacheService.UpdateUserAdventure(userId, userCachedAdventure, cancellationToken);

            _logger.LogStepInfo("ProcessUserAdventure method : Update user cached Adventure", "UpdateUserCachedAdventure()", userCachedAdventure.ToJson(), userId);


            if (!updatedSuccessfully)
            {
                _logger.UnableToUpdateUserCachedAdventure(userId);
                return null;
            }

            return nextNodes;
        }

        /// <summary>
        /// GetUserAdventureResult
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>AdventureTreeNode?</returns>
        public async Task<AdventureTreeNode?> GetUserAdventureResult(string userId, CancellationToken cancellationToken)
        {
            var userCachedAdventure = await _adventureCacheService.GetUserAdventure(userId, cancellationToken);
            _logger.LogStepInfo("GetUserAdventureResult method : Getting user cached adventure", "UserCachedAdventure", userCachedAdventure.ToJson(), userId);

            if (userCachedAdventure == null)
            {
                _logger.UserAdventureIsNotAvailable(userId);
                return null;
            }

            if (!userCachedAdventure.IsSelected)
            {
                _logger.AdventureRootNodeNotSelected();
                return new AdventureTreeNode(userCachedAdventure.NodeId, userCachedAdventure.NodeText);
            }

            ExtractSelectedSteps(userCachedAdventure);

            _logger.LogStepInfo("GetUserAdventureResult method : extract selected steps", "UserCachedAdventure", userCachedAdventure.ToJson(), userId);

            return userCachedAdventure;
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
                    return GetNextStep(qNode);
                }

                if (!qNode.IsSelected)
                    continue;

                if (qNode.LeftChild != null && !visited.Contains(qNode.LeftChild.NodeId))
                    nodeQueue.Enqueue(qNode.LeftChild);

                if (qNode.RightChild != null && !visited.Contains(qNode.RightChild.NodeId))
                    nodeQueue.Enqueue(qNode.RightChild);
            }

            return null;
        }

        private AdventureTreeNode? GetFirstChildNodes(AdventureTreeNode cachedUserAdventure)
        {
            if (cachedUserAdventure.NodeId == 0)
            {
                cachedUserAdventure.IsSelected = true;
                return GetNextStep(cachedUserAdventure);
            }

            return null;
        }

        private static AdventureTreeNode GetNextStep(AdventureTreeNode qNode)
        {
           var selectedNode = new AdventureTreeNode(qNode.NodeId, qNode.NodeText, true);

            if (qNode.LeftChild != null)
            {
                selectedNode.LeftChild = new AdventureTreeNode(qNode.LeftChild.NodeId, qNode.LeftChild.NodeText);
            }
            if (qNode.RightChild != null)
            {
                selectedNode.RightChild = new AdventureTreeNode(qNode.RightChild.NodeId, qNode.RightChild.NodeText);
            }

            return selectedNode;
        }
    }
}
