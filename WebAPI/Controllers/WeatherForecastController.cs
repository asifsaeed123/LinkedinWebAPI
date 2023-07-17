using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("linkedin")]
        public async Task<IActionResult> LinkedInAuth()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/weatherforecast/linkedin-callback",
                Parameters =
                {
                    { "scope", "r_liteprofile" }
                }
            };

            return Challenge(properties, "LinkedIn");
        }

        [HttpPost("linkedin-callback")]
        public async Task<IActionResult> LinkedInCallback()
        {
            var result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                // User authenticated successfully
                var accessToken = result.Properties.GetTokenValue("access_token");
                // Store the access token for future API calls

                return Ok();
            }
            else
            {
                // Authentication failed
                return BadRequest();
            }
        }


        [HttpGet("linkedin-callbackrest")]
        public async Task<ActionResult> LinkedInAPI()
        {
            // Handle the LinkedIn callback logic here
            // You will receive the authorization code from LinkedIn
            // Use the authorization code to obtain an access token

            // Example implementation:
            var authorizationCode = Request.Query["code"];

            // Make a request to the access token URL to obtain the access token
            // You can use a library like HttpClient to make the request

            // Example implementation:
            var httpClient = new HttpClient();
            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", authorizationCode },
                { "client_id", "77iqbgeznhzlh8" },
                { "client_secret", "xvRDrUBGfumODnxd" },
                { "redirect_uri", "https://localhost:5001/weatherforecast/linkedin-callback" }
            };

            var response = await httpClient.PostAsync("https://www.linkedin.com/oauth/v2/accessToken", new FormUrlEncodedContent(parameters));
            var content = await response.Content.ReadAsStringAsync();

            // Process the access token response and extract the access token

            // Example implementation:
            //var accessToken = JObject.Parse(content)["access_token"].ToString();

            // You can now use the access token to make authenticated requests to LinkedIn API or perform other actions

            return Ok();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
