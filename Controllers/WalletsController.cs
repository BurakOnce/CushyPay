using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Responses;
using CushyPay.Application.Features.Wallets.Commands.CreateWallet;
using CushyPay.Application.Features.Wallets.Queries.GetUserWallets;

namespace CushyPay.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WalletsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<WalletDto>> CreateWallet([FromBody] CreateWalletCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<WalletDto>>> GetUserWallets(int userId)
    {
        var query = new GetUserWalletsQuery { UserId = userId };
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}

