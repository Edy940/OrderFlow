using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTO;
using OrderFlow.Application.Interfaces;
using Asp.Versioning;

namespace OrderFlow.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    [AllowAnonymous, HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto dto)
    {
        var resultado = await _authService.RegistrarAsync(dto);
        return resultado.Sucesso ? Ok(resultado.Tokens) : Conflict(new { mensagem = resultado.Erro });
    }

    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var resultado = await _authService.LoginAsync(dto);
        return resultado.Sucesso ? Ok(resultado.Tokens) : Unauthorized(new { mensagem = resultado.Erro });
    }

    [AllowAnonymous, HttpPost("refresh")]
    public async Task<IActionResult> Renovar([FromBody] RenovarTokenDto dto)
    {
        var resultado = await _authService.RenovarAsync(dto.RefreshToken);
        return resultado.Sucesso ? Ok(resultado.Tokens) : Unauthorized(new { mensagem = resultado.Erro });
    }

    [AllowAnonymous, HttpPost("revogar")]
    public async Task<IActionResult> Revogar([FromBody] RevogarTokenDto dto)
    {
        var revogado = await _authService.RevogarAsync(dto.RefreshToken);
        return revogado ? NoContent() : BadRequest(new { mensagem = "Refresh token inválido ou inativo." });
    }
}
