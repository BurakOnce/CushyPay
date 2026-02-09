using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Responses;
using CushyPay.Application.Features.Transactions.Commands.Deposit;
using CushyPay.Application.Features.Transactions.Commands.ExternalTransfer;
using CushyPay.Application.Features.Transactions.Commands.InternalTransfer;
using CushyPay.Application.Features.Transactions.Commands.Withdraw;
using CushyPay.Application.Features.Transactions.Queries.GetTransactionHistory;

namespace CushyPay.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("internal-transfer")]
    public async Task<ActionResult<TransactionDto>> InternalTransfer([FromBody] InternalTransferCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("external-transfer")]
    public async Task<ActionResult<TransactionDto>> ExternalTransfer([FromBody] ExternalTransferCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("deposit")]
    public async Task<ActionResult<TransactionDto>> Deposit([FromBody] DepositCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("withdraw")]
    public async Task<ActionResult<TransactionDto>> Withdraw([FromBody] WithdrawCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("history/{walletId}")]
    public async Task<ActionResult<List<TransactionDto>>> GetTransactionHistory(
        int walletId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? type,
        [FromQuery] int? status,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount)
    {
        var query = new GetTransactionHistoryQuery
        {
            WalletId = walletId,
            FromDate = fromDate,
            ToDate = toDate,
            Type = type.HasValue ? (Domain.Enums.TransactionType?)type.Value : null,
            Status = status.HasValue ? (Domain.Enums.TransactionStatus?)status.Value : null,
            MinAmount = minAmount,
            MaxAmount = maxAmount
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}

