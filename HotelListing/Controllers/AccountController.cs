using System;
using System.Threading.Tasks;
using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

//TODO: AUTH
//sign in manager is not required for API because we are using TOKENS
namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApiUser> userManager,
            ILogger<AccountController> logger,
            IMapper mapper
        )
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }
        
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation($"Registration Attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = _mapper.Map<ApiUser>(userDTO);
                user.UserName = userDTO.Email;
                var result = await _userManager.CreateAsync(user, userDTO.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                await _userManager.AddToRolesAsync(user, userDTO.Roles);
                return Ok(new
                {
                    title = "Success",
                    message = "User registered successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong is the {nameof(Register)}");
                return StatusCode(500, new
                {
                    CustomeMessage = "Internal server error, something went wrong",
                    Message = ex.Message
                });
            }
        } 
        
        // [HttpPost]
        // [Route("login")]
        // public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        // {
        //     _logger.LogInformation($"Login Attempt for {userDTO.Email} {userDTO.Password}");
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     try
        //     {
        //         var result = await _signInManager.PasswordSignInAsync(
        //             userDTO.Email, userDTO.Password, false, false
        //             );
        //         if (!result.Succeeded)
        //         {
        //             return Unauthorized(userDTO);
        //         }
        //
        //         return Accepted();
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, $"Something went wrong is the {nameof(Login)}");
        //         return StatusCode(500, new
        //         {
        //             CustomMessage = "Internal server error, something went wrong",
        //             Message = ex.Message
        //         });
        //     }
        // } 
    }
}
