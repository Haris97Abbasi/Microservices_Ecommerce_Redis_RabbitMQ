using Catalog.Application.Dtos;
using Catalog.Application.Queries;
using Catalog.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("catalog/assistant")]
public class AssistantController(IMediator mediator, ILogger<AssistantController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<AssistantResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Ask(AssistantRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await mediator.Send(new AskAssistantQuery(request.Question), cancellationToken));
        }
        catch (LlmException ex)
        {
            logger.LogError(ex, "AI assistant call failed");
            return Problem(
                title: "AI assistant unavailable",
                detail: "The language model could not be reached. Check the Llm settings (BaseUrl, ApiKey, Model).",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }
}
