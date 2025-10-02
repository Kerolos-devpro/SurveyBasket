
using Microsoft.AspNetCore.OutputCaching;
using SurveyBasket.Api.Contracts.Votes;
namespace SurveyBasket.Api.Controllers;
[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize(Roles = $"{DefaultRoles.Member}")]
public class VotesController(IQuestionService questionService , IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteService _voteService = voteService;

    [HttpGet("")]
 
    public async Task<IActionResult> Start([FromRoute] int pollId , CancellationToken cancellationToken)
    {
        var userId = "eded301a-3497-4a3a-bc02-70877c50a89e";
        var result = await _questionService.GetAvailableAsync(pollId, userId! , cancellationToken);
        if(result.IsSuccess)
            return Ok(result.Value);


        return  result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _voteService.AddAsync(pollId,userId!, request, cancellationToken);

        return result.IsSuccess ? Created() : result.ToProblem();
    }








}
