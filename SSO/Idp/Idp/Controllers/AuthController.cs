using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Idp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] string SAMLRequest, [FromQuery] string RelayState)
        {
            _logger.LogInformation($"Login: SAMLRequest = {SAMLRequest}, RelayState = {RelayState}");
            
            //// Decode and validate SAMLRequest here (simplified for the example)
            var decodedSAMLRequest = Encoding.UTF8.GetString(Convert.FromBase64String(SAMLRequest));
            _logger.LogInformation($"Login: decodedSAMLRequest = {decodedSAMLRequest}, RelayState = {RelayState}");

            //// In a real app, store RelayState in TempData or another session state
            ////TempData["RelayState"] = RelayState;

            //// Serve the login page as a static HTML file (assuming it's served under wwwroot)
            //return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "login.html"), "text/html");

            // Redirect to login.html with SAMLRequest and RelayState as URL parameters
            var redirectUrl = $"/login.html?SAMLRequest={Uri.EscapeDataString(decodedSAMLRequest)}&RelayState={Uri.EscapeDataString(RelayState)}";
            
            _logger.LogInformation($"Login: redirectUrl = {redirectUrl}");
            
            return Redirect(redirectUrl);
        }

        [HttpPost("validate")]
        public IActionResult Validate([FromBody] LoginModel model)
        {
            _logger.LogInformation($"Validate: model = {System.Text.Json.JsonSerializer.Serialize(model)}");

            // Simplified user validation
            if (model.Username == "admin" && model.Password == "password")
            {
                // Generate SAMLResponse (in a real implementation, sign and validate it)
                var samlResponse = "<SAMLResponse>...</SAMLResponse>";
                var relayState = model.RelayState;

                // Encode SAMLResponse
                var encodedSAMLResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(samlResponse));

                // Return SAMLResponse and RelayState as JSON
                return Ok(new { SAMLResponse = encodedSAMLResponse, RelayState = relayState });
            }

            return Unauthorized(); // Invalid credentials
        }
    }


    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string SAMLRequest { get; set; }
        public string RelayState { get; set; }
    }
}
