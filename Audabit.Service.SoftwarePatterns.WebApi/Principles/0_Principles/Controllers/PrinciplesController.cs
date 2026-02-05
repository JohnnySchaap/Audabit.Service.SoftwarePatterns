using Asp.Versioning;
using Audabit.Service.SoftwarePatterns.App.Principles._0_Principles;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.SoftwarePatterns.WebApi.Principles._0_Principles.Controllers;

[Tags("0_Principles")]
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/principles")]
public sealed class PrinciplesController(IPrinciplesService principles) : ControllerBase
{
    [HttpGet("solid")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Solid()
    {
        principles.Solid();
        return Ok();
    }

    [HttpGet("dry")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Dry()
    {
        principles.Dry();
        return Ok();
    }

    [HttpGet("kiss")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Kiss()
    {
        principles.Kiss();
        return Ok();
    }

    [HttpGet("yagni")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Yagni()
    {
        principles.Yagni();
        return Ok();
    }

    [HttpGet("separation-of-concerns")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult SeparationOfConcerns()
    {
        principles.SeparationOfConcerns();
        return Ok();
    }

    [HttpGet("encapsulation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Encapsulation()
    {
        principles.Encapsulation();
        return Ok();
    }

    [HttpGet("composition-over-inheritance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CompositionOverInheritance()
    {
        principles.CompositionOverInheritance();
        return Ok();
    }
}