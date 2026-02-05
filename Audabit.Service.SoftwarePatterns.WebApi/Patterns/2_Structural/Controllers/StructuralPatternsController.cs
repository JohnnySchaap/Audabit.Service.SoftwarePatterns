using Asp.Versioning;
using Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.SoftwarePatterns.WebApi.Patterns._2_Structural.Controllers;

[Tags("2_StructuralPatterns")]
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/structuralPatterns")]
public sealed class StructuralPatternsController(IStructuralPatternsService structuralPatterns) : ControllerBase
{
    [HttpGet("1_adapter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAdapter()
    {
        structuralPatterns.Adapter();
        return Ok();
    }

    [HttpGet("2_bridge")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetBridge()
    {
        structuralPatterns.Bridge();
        return Ok();
    }

    [HttpGet("3_composite")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetComposite()
    {
        structuralPatterns.Composite();
        return Ok();
    }

    [HttpGet("4_decorator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetDecorator()
    {
        structuralPatterns.Decorator();
        return Ok();
    }

    [HttpGet("5_facade")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetFacade()
    {
        structuralPatterns.Facade();
        return Ok();
    }

    [HttpGet("6_flyweight")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetFlyweight()
    {
        structuralPatterns.Flyweight();
        return Ok();
    }

    [HttpGet("7_proxy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProxy()
    {
        structuralPatterns.Proxy();
        return Ok();
    }

    [HttpGet("8_markerInterface")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetMarkerInterface()
    {
        structuralPatterns.MarkerInterface();
        return Ok();
    }

    [HttpGet("9_pipesAndFilters")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetPipesAndFilters()
    {
        structuralPatterns.PipesAndFilters();
        return Ok();
    }
}