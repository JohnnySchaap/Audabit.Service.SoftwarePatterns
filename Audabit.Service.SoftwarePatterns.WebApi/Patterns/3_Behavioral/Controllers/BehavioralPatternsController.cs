using Asp.Versioning;
using Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.SoftwarePatterns.WebApi.Patterns._3_Behavioral.Controllers;

[Tags("3_BehavioralPatterns")]
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/behavioralPatterns")]
public sealed class BehavioralPatternsController(IBehavioralPatternsService behavioralPatterns) : ControllerBase
{
    [HttpGet("1_observer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetObserver()
    {
        behavioralPatterns.Observer();
        return Ok();
    }

    [HttpGet("2_strategy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetStrategy()
    {
        behavioralPatterns.Strategy();
        return Ok();
    }

    [HttpGet("3_command")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCommand()
    {
        behavioralPatterns.Command();
        return Ok();
    }

    [HttpGet("4_chainOfResponsibility")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetChainOfResponsibility()
    {
        behavioralPatterns.ChainOfResponsibility();
        return Ok();
    }

    [HttpGet("5_mediator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetMediator()
    {
        behavioralPatterns.Mediator();
        return Ok();
    }

    [HttpGet("6_memento")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetMemento()
    {
        behavioralPatterns.Memento();
        return Ok();
    }

    [HttpGet("7_state")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetState()
    {
        behavioralPatterns.State();
        return Ok();
    }

    [HttpGet("8_templateMethod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetTemplateMethod()
    {
        behavioralPatterns.TemplateMethod();
        return Ok();
    }

    [HttpGet("9_visitor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetVisitor()
    {
        behavioralPatterns.Visitor();
        return Ok();
    }

    [HttpGet("10_iterator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetIterator()
    {
        behavioralPatterns.Iterator();
        return Ok();
    }
}