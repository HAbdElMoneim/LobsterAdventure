using LobsterAdventure.Api.Services;
using LobsterAdventure.Core.Extentions;
using LobsterAdventure.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LobsterAdventure.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserAdventureController : ControllerBase
    {
        private readonly IUserAdventureService _userAdventureService;
        private readonly ILogger<UserAdventureController> _logger;

        public UserAdventureController(IUserAdventureService userAdventureService, ILogger<UserAdventureController> logger)
        {
            _userAdventureService = userAdventureService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint to start new user adventure
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>First step in the adventure</returns>
        [Authorize]
        [HttpGet, Route("start-adventure")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<AdventureTreeNode>> StartNewAdventure(CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = GetUser();
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.UnUnauthorizedUser();
                    return Unauthorized("Unauthorized User.");
                }

                var result = await _userAdventureService.CreateUserAdventure(userId, cancellationToken);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.UnexpectedError(ex);
                return NotFound();
            }
        }

        /// <summary>
        /// Endpoint to get user next step.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="CancellationToken"></param>
        /// <returns>Next adventure step.</returns>
        [Authorize]
        [HttpGet, Route("next-step")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<AdventureTreeNode>> GetAdventureNextStep([FromQuery] int nodeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = GetUser();
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.UnUnauthorizedUser();
                    return Unauthorized("Unauthorized User.");
                }

                if(nodeId <= 0)
                {
                    _logger.InvalidRequest("nodeId", nodeId.ToString());
                    return BadRequest("Node id must be greater than zero.");
                }

                var result = await _userAdventureService.ProcessUserAdventure(nodeId, userId, cancellationToken);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.UnexpectedError(ex);
                return NotFound();
            }
        }

        /// <summary>
        /// Endpoint to get User Adventure selections
        /// </summary>
        /// <returns>User adventure selected steps</returns>
        [Authorize]
        [HttpGet, Route("adventure-result")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<AdventureTreeNode>> GetAdventureResult(CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = GetUser();
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.UnUnauthorizedUser();
                    return Unauthorized("Unauthorized User.");
                }

                var result = await _userAdventureService.GetUserAdventureResult(userId, cancellationToken);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.UnexpectedError(ex);
                return NotFound();
            }
        }

        private string GetUser() => User.Claims.ToList().FirstOrDefault(x => x.Type == "UserId").Value;
    }
}