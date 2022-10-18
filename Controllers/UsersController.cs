using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using online_store_api.Data;

// Entity Framework Core
using Microsoft.EntityFrameworkCore;
using online_store_api.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace online_store_api.Controllers
{
    // Route Products
    [Route("[controller]")]
    // change the behaviour of the controller
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly ApplicationDbContext _dbContext;

        [Authorize(Roles = Authorization.Admin)]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserResponse>> CreateUser(CreateUserRequest request){
            request.Email = request.Email.ToLowerInvariant();

            var dbUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if(dbUser != null){
                ModelState.AddModelError(nameof(request.Email), "Email already exists");
                return ValidationProblem();
            }

            dbUser = new User{
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                FullName = request.FullName,
                Role = request.Role,
                IsActive = true
            };

            var hashResult = PasswordHelper.HashPassword(request.Password);

            dbUser.PasswordHash = hashResult.PasswordHash;
            dbUser.PasswordSalt = hashResult.PasswordSalt;

            await _dbContext.Users.AddAsync(dbUser);
            await _dbContext.SaveChangesAsync();

            var response = new UserResponse(dbUser);

            return Created("", response);
        }

        public UsersController(ApplicationDbContext dbContext)
        { 
            this._dbContext = dbContext;
        }

        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(403 )]
        [HttpGet("self")]
        public async Task<ActionResult<UserResponse>> GetSelf(){
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Forbid();

            var user = await _dbContext.Users.FindAsync(userId);
            if(user == null) return NotFound();

            var response = new UserResponse(user);

            return Ok(response);
        } 

        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(403 )]
        [HttpPut("self")]
        public async Task<ActionResult<UserResponse>> UpdateSelf(UpdateSelfRequest request){
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Forbid();

            var user = await _dbContext.Users.FindAsync(userId);
            if(user == null) return NotFound();

            user.Email = request.Email;
            user.FullName = request.FullName;

            if(!string.IsNullOrWhiteSpace(request.Password)){
                var hashResult = PasswordHelper.HashPassword(request.Password);
                user.PasswordHash = hashResult.PasswordHash;
                user.PasswordSalt = hashResult.PasswordSalt;
            }
            
            await _dbContext.SaveChangesAsync();


            var response = new UserResponse(user);

            return Ok(response);
        } 
    }

    public class UpdateSelfRequest{
        // Email, FullName, Password
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Password { get; set; }

    }

   

    public class CreateUserRequest{
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FullName { get; set; }
         // Role
        [Required]
        public string Role { get; set; }

        [Required]
        public string Password { get; set; }
    }

    // User Response with Id, Email, FullName, Role, Password
    public class UserResponse{
        public UserResponse(User user)
        {
            Id = user.Id;
            Email = user.Email;
            FullName = user.FullName;
            Role = user.Role;
            IsActive = user.IsActive;
            
        }
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }

        public bool IsActive { get; set; }
    }

}
