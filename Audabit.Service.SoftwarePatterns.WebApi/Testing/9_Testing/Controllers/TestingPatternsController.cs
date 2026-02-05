using Asp.Versioning;
using Audabit.Service.SoftwarePatterns.App.Testing._9_Testing;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.SoftwarePatterns.WebApi.Testing._9_Testing.Controllers;

[Tags("9_TestingPractices")]
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/testingPatterns")]
public sealed class TestingPatternsController(ITestingPatternsService testingPatterns) : ControllerBase
{
    [HttpGet("1_testPyramid")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetTestPyramid()
    {
        testingPatterns.TestPyramid();
        return Ok();
    }

    [HttpGet("2_mockStubFake")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetMockStubFake()
    {
        testingPatterns.MockStubFake();
        return Ok();
    }

    [HttpGet("3_tdd")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetTDD()
    {
        testingPatterns.TDD();
        return Ok();
    }

    [HttpGet("4_bdd")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetBDD()
    {
        testingPatterns.BDD();
        return Ok();
    }

    [HttpGet("5_goldenMaster")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetGoldenMaster()
    {
        testingPatterns.GoldenMaster();
        return Ok();
    }
}