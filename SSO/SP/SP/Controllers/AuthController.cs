using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Web;
using System.Xml;

namespace SP.Controllers
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
        public IActionResult Login()
        {
            // Generate SAMLRequest (simplified)
            var samlRequest = "<SAMLRequest>...</SAMLRequest>";  // This should be a valid SAML XML request
            var samlRequestBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(samlRequest));
            var relayState = "/designs.html"; // Optional RelayState to maintain session info

            // Redirect to IdP with SAMLRequest and RelayState
            //var redirectUrl = $"https://www.myidentityprovider.com/auth/login?SAMLRequest={HttpUtility.UrlEncode(samlRequestBase64)}&RelayState={HttpUtility.UrlEncode(relayState)}";
            var redirectUrl = $"https://localhost:7098/auth/login?SAMLRequest={HttpUtility.UrlEncode(samlRequestBase64)}&RelayState={HttpUtility.UrlEncode(relayState)}";

            return Redirect(redirectUrl);
        }

        [HttpPost("saml/acs")]
        public IActionResult ACS([FromForm] string SAMLResponse, [FromForm] string RelayState)
        {
            _logger.LogInformation($"saml/acs: SAMLResponse = {SAMLResponse}, RelayState: {RelayState}");

            // Process SAMLResponse (for simplicity, decode it here; in reality, verify and parse it)
            var decodedSAMLResponse = Encoding.UTF8.GetString(Convert.FromBase64String(SAMLResponse));
            _logger.LogInformation($"saml/acs: decodedSAMLResponse = {decodedSAMLResponse}");

            // Create a session or token for the user (using a cookie, for example)
            HttpContext.Response.Cookies.Append("authToken", "userTokenValue", new CookieOptions { HttpOnly = true });

            // Redirect the user to the original URL or homepage
            return Redirect(RelayState ?? "/index.html");
        }

    }
}
