using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LobsterAdventure.Core.Extentions
{
    public static class Logger
    {
        public static void UserAdventureIsNotAvailable(this ILogger logger, string userId) =>
            logger.LogWarning($"Adventure for user : {userId} is not available.");
        
        public static void UserAdventureAlreadyCached(this ILogger logger, string userId) =>
            logger.LogWarning($"Adventure for user : {userId} already cached.");

        public static void AdventureAlreadyCached(this ILogger logger) =>
            logger.LogWarning($"Cache already has an AdventureTree.");

        public static void RemoveExistingCachedAdventure(this ILogger logger) =>
            logger.LogWarning($"Remove existing cached adventure.");

        public static void AdventureTreeIsNotAvailable(this ILogger logger) =>
            logger.LogWarning($"AdventureTree is not yet available.");

        public static void AdventureArrayIsNullOrEmpty(this ILogger logger) =>
            logger.LogWarning($"Adventure array is null or empty."); 
        
        public static void AdventureRootNodeNotFound(this ILogger logger) =>
            logger.LogWarning($"Could not find Adventure root node.");

        public static void AdventureRootNodeNotSelected(this ILogger logger) =>
            logger.LogWarning($"Adventure root node is not selected.");
        
        public static void UserSelectedNodeIsNull(this ILogger logger, int nodeId, string userId) =>
            logger.LogWarning($"Could not find node id {nodeId} for user {userId} cached adventure tree.");
        
        public static void UnableToUpdateUserCachedAdventure(this ILogger logger, string userId) =>
            logger.LogWarning($"Could not update cached adventure for user {userId}.");

        public static void UnableToFindAnyCreatedAdventure(this ILogger logger) =>
           logger.LogWarning($"Could not Find any adventure in the system.");
        
        public static void UnUnauthorizedUser(this ILogger logger) =>
           logger.LogError($"Un authorized request.");
        
        public static void InvalidRequest(this ILogger logger, string name, string value) =>
           logger.LogError($"Invalid Request parameter {name} with value {value}");

        public static void UnexpectedError(this ILogger logger, Exception ex) =>
            logger.LogError($"Unexpected error : {ex}.");

        public static void LogStepInfo(this ILogger logger, string stepName, string objectName, string? objectData, string userId) =>
           logger.LogInformation($"For user {userId} - {stepName} with object Name {objectName}  and value {objectData ?? string.Empty}.");

        /// <summary>
        /// ToJson
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToJson(this object value)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        }
    }
}
