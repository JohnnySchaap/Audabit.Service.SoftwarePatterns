using Asp.Versioning;
using Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.SoftwarePatterns.WebApi.Patterns._1_Creational.Controllers;

[Tags("1_CreationalPatterns")]
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/creationalPatterns")]
public sealed class CreationalPatternsController(ICreationalPatternsService creationalPatterns) : ControllerBase
{
    [HttpGet("1_singleton")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSingleton()
    {
        creationalPatterns.Singleton();
        return Ok();
    }

    [HttpGet("2_lazyInitialization")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetLazyInitialization()
    {
        creationalPatterns.LazyInitialization();
        return Ok();
    }

    [HttpGet("3_simpleFactory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSimpleFactory()
    {
        creationalPatterns.SimpleFactory();
        return Ok();
    }

    [HttpGet("4_factoryMethod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetFactoryMethod()
    {
        creationalPatterns.FactoryMethod();
        return Ok();
    }

    [HttpGet("5_abstractFactory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAbstractFactory()
    {
        creationalPatterns.AbstractFactory();
        return Ok();
    }

    [HttpGet("6_builder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetBuilder()
    {
        creationalPatterns.Builder();
        return Ok();
    }

    [HttpGet("7_prototype")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetPrototype()
    {
        creationalPatterns.Prototype();
        return Ok();
    }

    [HttpGet("8_objectPool")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetObjectPool()
    {
        creationalPatterns.ObjectPool();
        return Ok();
    }

    [HttpGet("9_dependencyInjection")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetDependencyInjection()
    {
        creationalPatterns.DependencyInjection();
        return Ok();
    }
}