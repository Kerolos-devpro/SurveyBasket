using SurveyBasket.Api.Contracts;
using SurveyBasket.Api.Mapping;
using System.Diagnostics.Contracts;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService poll) : ControllerBase

{
    private readonly IPollService _pollService = poll;

    [HttpGet]
    public IActionResult GetAll()
    {
        var polls = _pollService.GetAll();
        return Ok(polls.MapToPollResponse());
    }

    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
       var poll = _pollService.Get(id); 
       return poll is null ? NotFound() : Ok(poll.MapToPollResponse()); 
    
    }

    [HttpPost("")]
    public IActionResult Add([FromBody] CreatePollRequest request)
    {
        var newPoll = _pollService.Add(request.MapToPoll());
        return CreatedAtAction(nameof(Get), new {  id = newPoll.Id },newPoll);
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromRoute]int id ,[FromBody] CreatePollRequest request)
    {
        var isUpdated = _pollService.Update(id , request.MapToPoll());
        if (isUpdated)
            return NoContent();

        return NotFound();
        
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id) { 
        var isDeleted = _pollService.Delete(id);
        if (isDeleted)
            return NoContent() ;

        return NotFound();
    }

}
