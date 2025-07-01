using Computational_Practice.DTOs;
using Computational_Practice.Services.Interfaces;
using Computational_Practice.Common;
using Computational_Practice.Common.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Computational_Practice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly ILogger<MatchesController> _logger;

        public MatchesController(IMatchService matchService, ILogger<MatchesController> logger)
        {
            _matchService = matchService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatches()
        {
            try
            {
                var matches = await _matchService.GetAllAsync();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні списку матчів");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResponse<MatchDto>>> GetPagedMatches([FromQuery] MatchFilter filter)
        {
            try
            {
                var matches = await _matchService.GetPagedAsync(filter, filter);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні списку матчів з пагінацією");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDto>> GetMatch(int id)
        {
            try
            {
                var match = await _matchService.GetByIdAsync(id);

                if (match == null)
                {
                    return NotFound($"Матч з ID {id} не знайдено");
                }

                return Ok(match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні матчу з ID {MatchId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<MatchDto>> GetMatchWithDetails(int id)
        {
            try
            {
                var match = await _matchService.GetWithDetailsAsync(id);

                if (match == null)
                {
                    return NotFound($"Матч з ID {id} не знайдено");
                }

                return Ok(match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні деталей матчу з ID {MatchId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("tournament/{tournamentId}")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByTournament(int tournamentId)
        {
            try
            {
                var matches = await _matchService.GetByTournamentAsync(tournamentId);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні матчів турніру {TournamentId}", tournamentId);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByTeam(int teamId)
        {
            try
            {
                var matches = await _matchService.GetByTeamAsync(teamId);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні матчів команди {TeamId}", teamId);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByPlayer(int playerId)
        {
            try
            {
                var matches = await _matchService.GetByPlayerAsync(playerId);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні матчів гравця {PlayerId}", playerId);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByStatus(string status)
        {
            try
            {
                var matches = await _matchService.GetByStatusAsync(status);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні матчів зі статусом {Status}", status);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("scheduled")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetScheduledMatches()
        {
            try
            {
                var matches = await _matchService.GetScheduledMatchesAsync();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні запланованих матчів");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("completed")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetCompletedMatches()
        {
            try
            {
                var matches = await _matchService.GetCompletedMatchesAsync();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні завершених матчів");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost]
        public async Task<ActionResult<MatchDto>> CreateMatch([FromBody] CreateMatchDto createDto)
        {
            try
            {
                var match = await _matchService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні матчу");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MatchDto>> UpdateMatch(int id, [FromBody] UpdateMatchDto updateDto)
        {
            try
            {
                var match = await _matchService.UpdateAsync(id, updateDto);
                if (match == null)
                {
                    return NotFound($"Матч з ID {id} не знайдено");
                }

                return Ok(match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні матчу з ID {MatchId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMatch(int id)
        {
            try
            {
                var result = await _matchService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"Матч з ID {id} не знайдено");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні матчу з ID {MatchId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost("{id}/start")]
        public async Task<ActionResult> StartMatch(int id)
        {
            try
            {
                var result = await _matchService.StartMatchAsync(id);
                if (!result)
                {
                    return BadRequest("Не вдалося розпочати матч");
                }

                return Ok("Матч успішно розпочато");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при початку матчу з ID {MatchId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult> CompleteMatch(int id, [FromBody] CompleteMatchRequest request)
        {
            try
            {
                var result = await _matchService.CompleteMatchAsync(id, request.WinnerTeamId, request.Result);
                if (!result)
                {
                    return BadRequest("Не вдалося завершити матч");
                }

                return Ok("Матч успішно завершено");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при завершенні матчу з ID {MatchId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelMatch(int id, [FromBody] CancelMatchRequest request)
        {
            try
            {
                var result = await _matchService.CancelMatchAsync(id, request.Reason);
                if (!result)
                {
                    return BadRequest("Не вдалося скасувати матч");
                }

                return Ok("Матч успішно скасовано");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при скасуvanні матчу з ID {MatchId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }
    }

    public class CompleteMatchRequest
    {
        public int? WinnerTeamId { get; set; }
        public string? Result { get; set; }
    }

    public class CancelMatchRequest
    {
        public string? Reason { get; set; }
    }
}
