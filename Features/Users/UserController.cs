using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApi.Shared.Contracts;

namespace UserApi.Features.Users;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService) => _userService = userService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User user)
    {
        var created = await _userService.CreateAsync(user);
        return StatusCode(201, new ApiResponse<User>(201, "User created successfully", created));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(new ApiResponse<List<User>>(200, "Users fetched successfully", users));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(new ApiResponse<User>(200, "User fetched successfully", user));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] User user)
    {
        var updated = await _userService.UpdateAsync(id, user);
        return Ok(new ApiResponse<User>(200, "User updated successfully", updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _userService.DeleteAsync(id);
        return Ok(new ApiResponse<object>(200, "User deleted successfully", null));
    }
}
