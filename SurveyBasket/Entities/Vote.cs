﻿namespace SurveyBasket.Api.Entities;

public sealed class Vote
{
    public int Id { get; set; }
    public int PollId { get; set; }
    public string UserId { get; set; }= string.Empty;
    public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;



    public Poll Poll { get; set; } = default!;
    public ApplicationUser ApplicationUser { get; set; } = default!;
    public ICollection<VoteAnswer> voteAnswers { get; set; } = [];
}
