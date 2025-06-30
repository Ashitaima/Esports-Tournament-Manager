using Computational_Practice.DTOs;
using Computational_Practice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Computational_Practice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(ITeamService teamService, ILogger<TeamsController> logger)
        {
            _teamService = teamService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams()
        {
            try
            {
                var teams = await _teamService.GetAllAsync();
                return Ok(teams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні списку команд");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(int id)
        {
            try
            {
                var team = await _teamService.GetByIdAsync(id);

                if (team == null)
                {
                    return NotFound($"Команду з ID {id} не знайдено");
                }

                return Ok(team);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні команди з ID {TeamId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("{id}/players")]
        public async Task<ActionResult<TeamDto>> GetTeamWithPlayers(int id)
        {
            try
            {
                var team = await _teamService.GetWithPlayersAsync(id);

                if (team == null)
                {
                    return NotFound($"Команду з ID {id} не знайдено");
                }

                return Ok(team);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні команди з гравцями з ID {TeamId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("tournament/{tournamentId}")]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeamsByTournament(int tournamentId)
        {
            try
            {
                var teams = await _teamService.GetByTournamentAsync(tournamentId);
                return Ok(teams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні команд для турніру {TournamentId}", tournamentId);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] CreateTeamDto createDto)
        {
            try
            {
                var team = await _teamService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні команди");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TeamDto>> UpdateTeam(int id, [FromBody] UpdateTeamDto updateDto)
        {
            try
            {
                var team = await _teamService.UpdateAsync(id, updateDto);
                if (team == null)
                {
                    return NotFound($"Команду з ID {id} не знайдено");
                }

                return Ok(team);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні команди з ID {TeamId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeam(int id)
        {
            try
            {
                var result = await _teamService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"Команду з ID {id} не знайдено");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні команди з ID {TeamId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost("{teamId}/players/{playerId}")]
        public async Task<ActionResult> AddPlayerToTeam(int teamId, int playerId)
        {
            try
            {
                var result = await _teamService.AddPlayerToTeamAsync(teamId, playerId);
                if (!result)
                {
                    return BadRequest("Не вдалося додати гравця до команди");
                }

                return Ok("Гравця успішно додано до команди");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при додаванні гравця {PlayerId} до команди {TeamId}", playerId, teamId);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpDelete("{teamId}/players/{playerId}")]
        public async Task<ActionResult> RemovePlayerFromTeam(int teamId, int playerId)
        {
            try
            {
                var result = await _teamService.RemovePlayerFromTeamAsync(teamId, playerId);
                if (!result)
                {
                    return BadRequest("Не вдалося видалити гравця з команди");
                }

                return Ok("Гравця успішно видалено з команди");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні гравця {PlayerId} з команди {TeamId}", playerId, teamId);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }
    }
}
