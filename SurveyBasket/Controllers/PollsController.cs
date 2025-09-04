

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Api.Contracts.Polls;
using System.Threading.Tasks;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService poll) : ControllerBase

{
    private readonly IPollService _pollService = poll;

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls =await _pollService.GetAllAsync(cancellationToken);

        var response = polls.Adapt<IEnumerable<PollResponse>>();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id , CancellationToken cancellationToken)
    {
        var poll = await _pollService.GetAsync(id , cancellationToken);
        if (poll == null)
            return NotFound();

        var response = poll.Adapt<PollResponse>();
        return Ok(response);

    }

    [HttpPost("")]
    public async Task<IActionResult> Add(
        [FromBody] PollRequest request, 
        [FromServices] IValidator<PollRequest> validator,
        CancellationToken cancellationToken
    )
    {

        var newPoll =await _pollService.AddAsync(request.Adapt<Poll>() , cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request , CancellationToken cancellationToken)
    {
        var isUpdated =await _pollService.UpdateAsync(id, request.Adapt<Poll>() ,cancellationToken);
        if (isUpdated)
            return NoContent();

        return NotFound();

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id , CancellationToken cancellationToken)
    {
        var isDeleted =await _pollService.DeleteAsync(id , cancellationToken);
        if (isDeleted)
            return NoContent();

        return NotFound();
    }

    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
        if (isUpdated)
            return NoContent();

        return NotFound();

    }


}
