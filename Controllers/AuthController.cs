using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using online_store_api.Data;
using online_store_api.Helpers;

namespace online_store_api.Controllers
{
    // Route Products
    [Route("[controller]")]
    // change the behaviour of the controller
    [ApiController]

    public class AuthController : ControllerBase {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthOptions _options;

        public AuthController(ApplicationDbContext dbContext, IOptions<AuthOptions> options)
        {
            _dbContext = dbContext;
            _options = options.Value;
        }
  
     
        [HttpPost("token")]
        public async Task<ActionResult<TokenResponse>> Login([FromForm]LoginRequest request){

            var email = request.Username.ToLowerInvariant();
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if(user == null) {
                return Unauthorized();
            }

            if(user.IsActive == false){
                return Forbid();
            }

            var hashResult = PasswordHelper.HashPassword(request.Password, user.PasswordSalt);
            if(hashResult.PasswordHash != user.PasswordHash){
                return Unauthorized();
            }

            // check for username and pass
            // if(request.Username != "user" || request.Password != "123"){
            //     return Unauthorized();
            // }

            // var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Secret));

            // var myIssuer = "http://mysite.com";
            // var myAudience = "http://myaudience.com";

            var lifetime = TimeSpan.FromHours(8);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.Add(lifetime),
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                // issuedAt = DateTime.UtcNow,
		        SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
	        };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenResponse{
                AccessToken = tokenHandler.WriteToken(token),
                ExpiresIn = (int)lifetime.TotalSeconds,
                TokenType = "Bearer"
            };
             
        }
        
    }

    public class LoginRequest {
     
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class TokenResponse{

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }

    

  




   
    
}