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
    public class AdventureController : ControllerBase
    {
        private readonly IAdventureService _adventureService;
        private readonly ILogger<AdventureController> _logger;

        public AdventureController(IAdventureService adventureService, ILogger<AdventureController> logger)
        {
            _adventureService = adventureService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint to Create Adventure
        /// </summary>
        /// <param name="adventureArray"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>bool</returns>
        [Authorize]
        [HttpPost, Route("CreateNew")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAdventure(string?[] adventureArray, CancellationToken cancellationToken = default)
        {
            try
            {
                if (adventureArray.Length <= 0)
                {
                    _logger.UnUnauthorizedUser();
                    return BadRequest("adventure must contains some steps.");
                }

                var result = await _adventureService.AddNewAdventure(adventureArray, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.UnexpectedError(ex);
                return BadRequest();
            }
        }

        /// <summary>
        /// Endpoint to get AdventureSteps
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="CancellationToken"></param>
        /// <returns>List of shows including list of casts for each show</returns>
        [Authorize]
        [HttpGet, Route("UserSteps")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<AdventureTreeNode>> GetUserAdventureSteps([FromQuery] int nodeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = User.Claims.ToList().FirstOrDefault(x => x.Type == "UserId").Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.UnUnauthorizedUser();
                    return Unauthorized("Unauthorized User.");
                }

                if(nodeId < 0)
                {
                    _logger.InvalidRequest("nodeId", nodeId.ToString());
                    return BadRequest("Node id must be greater than or equal zero.");
                }
                var result = await _adventureService.ProcessUserAdventure(nodeId, userId, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.UnexpectedError(ex);
                return NotFound();
            }
        }

        /// <summary>
        /// Endpoint to get shows User Adventure selections
        /// </summary>
        /// <returns>Adventure Selected steps</returns>
        [Authorize]
        [HttpGet, Route("Result")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<AdventureTreeNode>> GetUserAdventureResult()
        {
            try
            {
                var userId = User.Claims.ToList().FirstOrDefault(x => x.Type == "UserId").Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.UnUnauthorizedUser();
                    return Unauthorized("Unauthorized User.");
                }

                var result = await _adventureService.GetUserAdventureResult(userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.UnexpectedError(ex);
                return NotFound();
            }
        }
    }
}