using Asp.Versioning;
using Audabit.Service.SoftwarePatterns.App.Patterns._8_AntiPatterns;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.SoftwarePatterns.WebApi.Patterns._8_AntiPatterns.Controllers;

[Tags("8_AntiPatterns")]
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/antiPatterns")]
public sealed class AntiPatternsController(IAntiPatternsService antiPatterns) : ControllerBase
{
    [HttpGet("1_spaghettiCode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSpaghettiCode()
    {
        antiPatterns.SpaghettiCode();
        return Ok();
    }

    [HttpGet("2_godObject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetGodObject()
    {
        antiPatterns.GodObject();
        return Ok();
    }

    [HttpGet("3_goldenHammer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetGoldenHammer()
    {
        antiPatterns.GoldenHammer();
        return Ok();
    }

    [HttpGet("4_reinventingTheWheel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetReinventingTheWheel()
    {
        antiPatterns.ReinventingTheWheel();
        return Ok();
    }

    [HttpGet("5_copyPasteProgramming")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCopyPasteProgramming()
    {
        antiPatterns.CopyPasteProgramming();
        return Ok();
    }
}