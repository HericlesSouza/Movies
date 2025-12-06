using MediatR;

using Microsoft.AspNetCore.Mvc;

using Movies.API.Contracts.Requests;
using Movies.Application.Movies.Commands.CreateMovie;
using Movies.Application.Movies.Queries.GetMovieById;

namespace Movies.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateMovieRequest request,
        CancellationToken ct)
    {
        var command = new CreateMovieCommand(
            request.Title,
            request.Description,
            request.DurationInMinutes,
            request.Price
            );

        var result = await _sender.Send(command, ct);

        return CreatedAtAction(nameof(GetByIdAsync), result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var query = new GetMovieByIdQuery(id);

        var result = await _sender.Send(query, ct);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
