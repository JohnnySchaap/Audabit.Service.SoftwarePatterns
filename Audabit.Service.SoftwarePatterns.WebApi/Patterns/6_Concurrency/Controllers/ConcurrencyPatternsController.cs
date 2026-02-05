using Asp.Versioning;
using Audabit.Service.SoftwarePatterns.App.Patterns._6_Concurrency;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.SoftwarePatterns.WebApi.Patterns._6_Concurrency.Controllers;

[Tags("6_ConcurrencyPatterns")]
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/concurrencyPatterns")]
public sealed class ConcurrencyPatternsController(IConcurrencyPatternsService concurrencyPatterns) : ControllerBase
{
    [HttpGet("1_threadPool")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetThreadPoolAsync()
    {
        await concurrencyPatterns.ThreadPoolAsync();
        return Ok();
    }

    [HttpGet("2_futurePromise")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFuturePromiseAsync()
    {
        await concurrencyPatterns.FuturePromiseAsync();
        return Ok();
    }

    [HttpGet("3_readWriteLock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReadWriteLockAsync()
    {
        await concurrencyPatterns.ReadWriteLockAsync();
        return Ok();
    }

    [HttpGet("4_producerConsumer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducerConsumerAsync()
    {
        await concurrencyPatterns.ProducerConsumerAsync();
        return Ok();
    }

    [HttpGet("5_activeObject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveObjectAsync()
    {
        await concurrencyPatterns.ActiveObjectAsync();
        return Ok();
    }
}