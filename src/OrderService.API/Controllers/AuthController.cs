using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
            return Ok(new { message = "Kullanıcı başarıyla oluşturuldu." });

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _userManager .FindByNameAsync(loginDto.Username);

        if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            //token üret
            var token = _tokenService.GenerateToken(user.UserName, "User");
            return Ok(new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName
            });
        }

        return Unauthorized("Hatalı kullanıcı adı ya da şifresi");
    }
}