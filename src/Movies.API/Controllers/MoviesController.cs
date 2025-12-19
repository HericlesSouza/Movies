using MediatR;

using Microsoft.AspNetCore.Mvc;

using Movies.API.Contracts.Queries;
using Movies.API.Contracts.Requests;
using Movies.Application.Movies.Commands.CreateMovie;
using Movies.Application.Movies.Commands.DeleteMovie;
using Movies.Application.Movies.Commands.UpdateMovie;
using Movies.Application.Movies.Queries.GetMovieById;
using Movies.Application.Movies.Queries.ListMovies;

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
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetMoviesQuery moviesQuery, CancellationToken ct)
    {
        var query = new ListMoviesQuery(
            moviesQuery.Page,
            moviesQuery.PageSize,
            moviesQuery.Search,
            moviesQuery.SortBy,
            moviesQuery.SortDirection
            );

        var result = await _sender.Send(query, ct);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        [FromBody] UpdateMovieRequest request,
        Guid id,
        CancellationToken ct)
    {
        var command = new UpdateMovieCommand(
            id,
            request.Title,
            request.Description,
            request.DurationInMinutes,
            request.Price
            );

        var result = await _sender.Send(command, ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var command = new DeleteMovieCommand(id);

        var result = await _sender.Send(command, ct);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
