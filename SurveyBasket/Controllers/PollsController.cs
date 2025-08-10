

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

        var response = polls.Adapt<IEnumerable<PollResponse>>();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
       var poll = _pollService.Get(id);
        if (poll == null)
            return NotFound();

        var response = poll.Adapt<PollResponse>();
       return  Ok(response); 
    
    }

    [HttpPost("")]
    public IActionResult Add([FromBody] CreatePollRequest request)
    {
        var newPoll = _pollService.Add(request.Adapt<Poll>());
        return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);

        
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromRoute]int id ,[FromBody] CreatePollRequest request)
    {
        var isUpdated = _pollService.Update(id, request.Adapt<Poll>());
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

    [HttpPost("test")]
    public IActionResult Test([FromBody] Student request)
    {
        return Ok("Values are accepted");
    }

}
