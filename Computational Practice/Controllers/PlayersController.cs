using Computational_Practice.DTOs;
using Computational_Practice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Computational_Practice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(IPlayerService playerService, ILogger<PlayersController> logger)
        {
            _playerService = playerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers()
        {
            try
            {
                var players = await _playerService.GetAllAsync();
                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні списку гравців");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetPlayer(int id)
        {
            try
            {
                var player = await _playerService.GetByIdAsync(id);

                if (player == null)
                {
                    return NotFound($"Гравця з ID {id} не знайдено");
                }

                return Ok(player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні гравця з ID {PlayerId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("{id}/team")]
        public async Task<ActionResult<PlayerDto>> GetPlayerWithTeam(int id)
        {
            try
            {
                var player = await _playerService.GetWithTeamAsync(id);

                if (player == null)
                {
                    return NotFound($"Гравця з ID {id} не знайдено");
                }

                return Ok(player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні гравця з командою з ID {PlayerId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("{id}/matches")]
        public async Task<ActionResult<PlayerDto>> GetPlayerWithMatches(int id)
        {
            try
            {
                var player = await _playerService.GetWithMatchesAsync(id);

                if (player == null)
                {
                    return NotFound($"Гравця з ID {id} не знайдено");
                }

                return Ok(player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні гравця з матчами з ID {PlayerId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayersByTeam(int teamId)
        {
            try
            {
                var players = await _playerService.GetByTeamAsync(teamId);
                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні гравців команди {TeamId}", teamId);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("free-agents")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetFreeAgents()
        {
            try
            {
                var players = await _playerService.GetFreeAgentsAsync();
                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні вільних агентів");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> CreatePlayer([FromBody] CreatePlayerDto createDto)
        {
            try
            {
                var player = await _playerService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні гравця");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PlayerDto>> UpdatePlayer(int id, [FromBody] UpdatePlayerDto updateDto)
        {
            try
            {
                var player = await _playerService.UpdateAsync(id, updateDto);
                if (player == null)
                {
                    return NotFound($"Гравця з ID {id} не знайдено");
                }

                return Ok(player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні гравця з ID {PlayerId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlayer(int id)
        {
            try
            {
                var result = await _playerService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"Гравця з ID {id} не знайдено");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні гравця з ID {PlayerId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost("{playerId}/join-team/{teamId}")]
        public async Task<ActionResult> JoinTeam(int playerId, int teamId)
        {
            try
            {
                var result = await _playerService.JoinTeamAsync(playerId, teamId);
                if (!result)
                {
                    return BadRequest("Не вдалося приєднатися до команди");
                }

                return Ok("Гравець успішно приєднався до команди");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при приєднанні гравця {PlayerId} до команди {TeamId}", playerId, teamId);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost("{playerId}/leave-team")]
        public async Task<ActionResult> LeaveTeam(int playerId)
        {
            try
            {
                var result = await _playerService.LeaveTeamAsync(playerId);
                if (!result)
                {
                    return BadRequest("Не вдалося покинути команду");
                }

                return Ok("Гравець успішно покинув команду");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при залишенні команди гравцем {PlayerId}", playerId);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }
    }
}
