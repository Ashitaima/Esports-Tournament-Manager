using Computational_Practice.DTOs;
using Computational_Practice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Computational_Practice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні списку користувачів");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);

                if (user == null)
                {
                    return NotFound($"Користувача з ID {id} не знайдено");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні користувача з ID {UserId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userService.GetByUsernameAsync(username);

                if (user == null)
                {
                    return NotFound($"Користувача з іменем {username} не знайдено");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні користувача з іменем {Username}", username);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);

                if (user == null)
                {
                    return NotFound($"Користувача з email {email} не знайдено");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні користувача з email {Email}", email);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createDto)
        {
            try
            {
                var user = await _userService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні користувача");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                var user = await _userService.UpdateAsync(id, updateDto);
                if (user == null)
                {
                    return NotFound($"Користувача з ID {id} не знайдено");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні користувача з ID {UserId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"Користувача з ID {id} не знайдено");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні користувача з ID {UserId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost("{id}/activate")]
        public async Task<ActionResult> ActivateUser(int id)
        {
            try
            {
                var result = await _userService.ActivateAsync(id);
                if (!result)
                {
                    return NotFound($"Користувача з ID {id} не знайдено");
                }

                return Ok("Користувача успішно активовано");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при активації користувача з ID {UserId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult> DeactivateUser(int id)
        {
            try
            {
                var result = await _userService.DeactivateAsync(id);
                if (!result)
                {
                    return NotFound($"Користувача з ID {id} не знайдено");
                }

                return Ok("Користувача успішно деактивовано");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при деактивації користувача з ID {UserId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }
    }
}
