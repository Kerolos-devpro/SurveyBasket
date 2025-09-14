﻿

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Api.Contracts.Polls;
using System.Threading.Tasks;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService poll) : ControllerBase

{
    private readonly IPollService _pollService = poll;

    [HttpGet]
  
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls =await _pollService.GetAllAsync(cancellationToken);

        var response = polls.Adapt<IEnumerable<PollResponse>>();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id , CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id , cancellationToken);
    
        return result.IsSuccess 
            ? Ok(result.Value) 
            : Problem(statusCode: StatusCodes.Status404NotFound , title:result.Error.Code , detail: result.Error.Description );
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _pollService.AddAsync(request, cancellationToken);

        return result.IsSuccess ?
             CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value)
            : result.ToProblem(StatusCodes.Status409Conflict); 

        
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var result  = await _pollService.UpdateAsync(id, request, cancellationToken);

        if (result.IsSuccess)
            return NoContent();

        return result.Error.Equals(PollErrors.DuplicatedTitle)
                ? result.ToProblem(StatusCodes.Status409Conflict)
                : result.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ?
            NoContent()
            : result.ToProblem(StatusCodes.Status404NotFound);

    }

    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
       
        return result.IsSuccess ? 
            NoContent()
            :result.ToProblem(StatusCodes.Status404NotFound) ;

    }


}
