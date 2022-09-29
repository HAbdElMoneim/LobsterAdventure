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
    public class InternalAdventureController : ControllerBase
    {
        private readonly IAdventureService _adventureService;
        private readonly ILogger<InternalAdventureController> _logger;

        public InternalAdventureController(IAdventureService adventureService, ILogger<InternalAdventureController> logger)
        {
            _adventureService = adventureService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint to create new adventure
        /// </summary>
        /// <param name="adventureArray"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Bool indicate if adventure has been created successfully or not.</returns>
        [Authorize]
        [HttpPost, Route("create-adventure")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateNewAdventure(string?[] adventureArray, CancellationToken cancellationToken = default)
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
        /// Endpoint to get the current adventure
        /// </summary>
        /// <param name="CancellationToken"></param>
        /// <returns>Full adventure steps. </returns>
        [Authorize]
        [HttpGet, Route("current-adventure")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<AdventureTreeNode>> GetAdventure(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _adventureService.GetAdventure(cancellationToken);

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
    }
}