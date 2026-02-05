using Asp.Versioning;
using Audabit.Service.SoftwarePatterns.App.Patterns._7_Reliability;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.SoftwarePatterns.WebApi.Patterns._7_Reliability.Controllers;

[Tags("7_ReliabilityPatterns")]
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/reliabilityPatterns")]
public sealed class ReliabilityPatternsController(IReliabilityPatternsService reliabilityPatterns) : ControllerBase
{
    [HttpGet("1_circuitBreaker")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCircuitBreakerAsync()
    {
        await reliabilityPatterns.CircuitBreakerAsync();
        return Ok();
    }

    [HttpGet("2_bulkhead")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBulkheadAsync()
    {
        await reliabilityPatterns.BulkheadAsync();
        return Ok();
    }

    [HttpGet("3_retryBackoff")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRetryBackoffAsync()
    {
        await reliabilityPatterns.RetryBackoffAsync();
        return Ok();
    }

    [HttpGet("4_rateLimiting")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRateLimitingAsync()
    {
        await reliabilityPatterns.RateLimitingAsync();
        return Ok();
    }

    [HttpGet("5_failFast")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetFailFast()
    {
        reliabilityPatterns.FailFast();
        return Ok();
    }
}