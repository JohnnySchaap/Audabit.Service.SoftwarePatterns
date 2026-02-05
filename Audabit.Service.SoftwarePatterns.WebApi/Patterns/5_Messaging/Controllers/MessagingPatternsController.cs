using Asp.Versioning;
using Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Audabit.Service.SoftwarePatterns.WebApi.Patterns._5_Messaging.Controllers;

[Tags("5_MessagingPatterns")]
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/messagingPatterns")]
public sealed class MessagingPatternsController(IMessagingPatternsService messagingPatterns) : ControllerBase
{
    [HttpGet("1_outbox")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetOutbox()
    {
        messagingPatterns.Outbox();
        return Ok();
    }

    [HttpGet("2_inbox")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetInbox()
    {
        messagingPatterns.Inbox();
        return Ok();
    }

    [HttpGet("3_splitter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSplitter()
    {
        messagingPatterns.Splitter();
        return Ok();
    }

    [HttpGet("4_aggregator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAggregator()
    {
        messagingPatterns.Aggregator();
        return Ok();
    }

    [HttpGet("5_messageRouter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetMessageRouter()
    {
        messagingPatterns.MessageRouter();
        return Ok();
    }

    [HttpGet("6_contentBasedRouter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetContentBasedRouter()
    {
        messagingPatterns.ContentBasedRouter();
        return Ok();
    }

    [HttpGet("7_messageTranslator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetMessageTranslator()
    {
        messagingPatterns.MessageTranslator();
        return Ok();
    }

    [HttpGet("8_publishSubscribe")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetPublishSubscribe()
    {
        messagingPatterns.PublishSubscribe();
        return Ok();
    }

    [HttpGet("9_requestReply")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetRequestReply()
    {
        messagingPatterns.RequestReply();
        return Ok();
    }

    [HttpGet("10_deadLetterChannel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetDeadLetterChannel()
    {
        messagingPatterns.DeadLetterChannel();
        return Ok();
    }
}