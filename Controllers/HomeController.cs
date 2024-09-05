using Injazat.Presentation.Services.ErrorService;
using Injazat.Presentation.Services.LogDBService;
using Injazat.Presentation.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Diagnostics;

namespace Injazat.Presentation.Controllers
{
    [Route("[controller]")]
    [ServiceFilter(typeof(BaseController))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IErrorLogManager _errorLogManager;
        private readonly ILogDbService _logDbService;
        private readonly IUserService _userService;
      //  private readonly IErrorLogManager _errorLogManager;
        public HomeController(ILogger<HomeController> logger, ILogDbService logDbService, IUserService userService, IErrorLogManager errorLogManager, ILogger<BaseController> baseLogger)
        {
            _logger = logger;
            _logDbService = logDbService;
            _userService = userService;
            _errorLogManager = errorLogManager;
        }
        [HttpGet("throw")]
        public IActionResult ThrowException()
        {

            throw new InvalidOperationException("Anotger Exception accur in Home ");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            _logger.LogInformation("Handling Index request");

            if (Request.Headers["JsonPlease"].ToString().Contains("application/json"))
            {
                var apiResponse = new
                {
                    Message = "This is the API response for the Index action.",
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };

                return Ok("ello"); // Return JSON response for API requests
            }

            return View(); // Return the view for browser requests
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("Handling GetUsers request");

            try
            {
                // Await the asynchronous call to get the user
                var user = await _userService.GetUserByEmailAsync("ahmadnaseem1206@gmail.com");
  
                if (user != null)
                {
                    // Serialize and return the user object if found

                    return Ok(user);
                }
                else
                {
                    // Handle the case where no user is found
                  //  throw new Exception("User with email  not found.");
                  return NotFound("User not found.");
                }
            }
            catch (Exception ex)
            {
                //  throw new Exception("User with email  not found.");
              //  Log and handle any exceptions that occur during the process
                _logger.LogError($"An error occurred while retrieving user: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
