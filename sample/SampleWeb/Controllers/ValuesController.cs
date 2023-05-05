using System;
using System.Diagnostics;
using Exceptionless.Models;
using Exceptionless.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace SampleWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
           _logger.LogInformation("Get was called");
            return $"[{Activity.Current?.Id}] {User.Identity?.Name}";
        }

        [HttpGet("advanced-topic-user")]
        public string AdvancedTopicUser()
        {
            // This call is is authenticated so a user identity would automatically be set.
            // However we are overriding it with our own custom user. You may want to do this
            // in a microservice where you know the user but you may not be authenticated.
            using (LogContext.PushProperty(Event.KnownDataKeys.UserInfo, new UserInfo(User.Identity?.Name + " Custom", "Test User Full Name"), true)) {
                _logger.LogInformation("This log event will have a custom user set.");
            }

            return $"[{Activity.Current?.Id}] {User.Identity?.Name}";
        }

        [HttpGet("advanced-topic-user-description")]
        public string AdvancedTopicUserDescription(string description)
        {
            // User descriptions was intended to provide a description from an end user why an error happened.
            using (LogContext.PushProperty(Event.KnownDataKeys.UserDescription, new UserDescription(User.Identity?.Name, description), true)) {
                _logger.LogError(new Exception("Test"), "This error event will have a user description set on it.");
            }

            return $"[{Activity.Current?.Id}] {User.Identity?.Name}";
        }
    }
}