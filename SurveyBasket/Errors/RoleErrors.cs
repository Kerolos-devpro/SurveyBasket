namespace SurveyBasket.Api.Errors;

public class RoleErrors
{
    public static readonly Error RoleNotFound =
       new("Role.RoleNotFound", "Role is not found", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedRoleName =
       new("Role.DuplicatedRoleName", "Another role with the same name is already exists", StatusCodes.Status404NotFound);


    public static readonly Error InvalidPermissions =
       new("Role.InvalidPermissions", "Permissions are not from the allowed values", StatusCodes.Status404NotFound);

}
