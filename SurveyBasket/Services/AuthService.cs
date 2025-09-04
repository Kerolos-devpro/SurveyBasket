using SurveyBasket.Api.Authentication;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {

        //check user
        var user = await  _userManager.FindByEmailAsync(email);

        if (user == null)
            return null;


        // check password 
        var result = await _userManager.CheckPasswordAsync(user , password);

        if (!result)
            return null;





        // generate jwt token
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);

        // return auth response
        return new AuthResponse(user.Id, user.Email , user.FirstName , user.LastName , token , expiresIn);
    }
}
