using MediatR;

using Microsoft.AspNetCore.Mvc;

using Movies.API.Contracts.Requests;
using Movies.Application.Commands.CreateMovie;

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

        return CreatedAtAction(nameof(GetById), result);
    }

    [HttpGet]
    public IActionResult GetById()
    {
        return Ok("Movies API is running...");
    }
}
