using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TacticView.Data;

namespace TacticView.Api;

[ApiController]
[AllowAnonymous]
[Route("refresh-cache")]
public class CacheController : Controller
{
    private ILogger _log;
    private IConfiguration _config;
    private IWebHostEnvironment _env;

    public CacheController(ILogger<CacheController> logger, IConfiguration configuration, IWebHostEnvironment webHost)
    {
        _log = logger;
        _config = configuration;
        _env = webHost;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        GitHubQueryService github = new GitHubQueryService(_config, _env);

        // get consider
        await github.GetReposAndIssuesAsync("servicing-consider");
        _log.LogInformation("Retreived 'servicing-consider'");

        // get approved
        await github.GetReposAndIssuesAsync("servicing-approved", false);
        _log.LogInformation("Retreived 'servicing-approved'");

        return Ok();
    }
}
