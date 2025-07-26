﻿namespace SurveyBasket.Api.Contracts.Requests;

public class CreatePollRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public static explicit operator Poll(CreatePollRequest request)
    {
        return new () {
            Title = request.Title,
            Description = request.Description,
        };

    }
}
