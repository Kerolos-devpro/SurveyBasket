using System.ComponentModel.DataAnnotations;

namespace SurveyBasket.Api.Contracts.Requests;

public record CreatePollRequest(
    [Required(ErrorMessage = "This field is so important so add it please")]
    [Length(3 , 50 , ErrorMessage = "Title should be between 3 and 50 character")]
    [AllowedValues("New" , "Old" , "Pasaa" , ErrorMessage ="The allowed values are : (Old , New) only!")]
    string Title,
    string Description
    );