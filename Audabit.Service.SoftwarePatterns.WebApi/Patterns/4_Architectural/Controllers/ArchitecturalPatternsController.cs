using Asp.Versioning;
using Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.SoftwarePatterns.WebApi.Patterns._4_Architectural.Controllers;

[Tags("4_ArchitecturalPatterns")]
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/architecturalPatterns")]
public sealed class ArchitecturalPatternsController(IArchitecturalPatternsService architecturalPatterns) : ControllerBase
{
    [HttpGet("1_layered")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetLayered()
    {
        architecturalPatterns.Layered();
        return Ok();
    }

    [HttpGet("2_hexagonal")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetHexagonal()
    {
        architecturalPatterns.Hexagonal();
        return Ok();
    }

    [HttpGet("3_microservices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetMicroservices()
    {
        architecturalPatterns.Microservices();
        return Ok();
    }

    [HttpGet("4_eventDriven")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetEventDriven()
    {
        architecturalPatterns.EventDriven();
        return Ok();
    }

    [HttpGet("5_cqrs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCQRS()
    {
        architecturalPatterns.CQRS();
        return Ok();
    }

    [HttpGet("6_eventSourcing")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetEventSourcing()
    {
        architecturalPatterns.EventSourcing();
        return Ok();
    }

    [HttpGet("7_saga")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSaga()
    {
        architecturalPatterns.Saga();
        return Ok();
    }

    [HttpGet("8_apiGateway")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetApiGateway()
    {
        architecturalPatterns.ApiGateway();
        return Ok();
    }

    [HttpGet("9_stranglerFig")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetStranglerFig()
    {
        architecturalPatterns.StranglerFig();
        return Ok();
    }
}