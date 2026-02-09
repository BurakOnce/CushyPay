using MediatR;
using Microsoft.AspNetCore.Mvc;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Responses;
using CushyPay.Application.Features.Users.Commands.LoginUser;
using CushyPay.Application.Features.Users.Commands.RefreshToken;
using CushyPay.Application.Features.Users.Commands.RegisterUser;

namespace CushyPay.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginUserCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Unauthorized(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Unauthorized(result.Error);

        return Ok(result.Value);
    }
}

