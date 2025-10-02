namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]

public class PollsController(IPollService poll) : ControllerBase

{
    private readonly IPollService _pollService = poll;

    [HttpGet("")]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _pollService.GetAllAsync(cancellationToken));
    }

    [Authorize(Roles = $"{DefaultRoles.Member}")]
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        return Ok(await _pollService.GetCurrentAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> Get([FromRoute] int id , CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id , cancellationToken);
    
        return result.IsSuccess 
            ? Ok(result.Value) 
            : result.ToProblem();
    }

    [HttpPost("")]
    [HasPermission(Permissions.AddPolls)]
    public async Task<IActionResult> Add([FromBody] PollRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _pollService.AddAsync(request, cancellationToken);

        return result.IsSuccess ?
             CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value)
            : result.ToProblem(); 

        
    }


    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var result  = await _pollService.UpdateAsync(id, request, cancellationToken);

        if (result.IsSuccess)
            return NoContent();

      
        return result.ToProblem();
    }

    [HasPermission(Permissions.DeletePolls)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ?
            NoContent()
            : result.ToProblem();

    }

    [HasPermission(Permissions.UpdatePolls)]
    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
       
        return result.IsSuccess ? 
            NoContent()
            :result.ToProblem() ;

    }


}
