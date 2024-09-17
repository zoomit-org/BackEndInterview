using Microsoft.AspNetCore.Mvc;
using Weblog.Application.Commands;
using Weblog.Application.Services;

namespace Weblog.Api.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var result = _articleService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    public IActionResult Create(CreateArticleCommand command)
    {
        var result = _articleService.Create(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public IActionResult Publish(string id, PublishArticleCommand command)
    {
        command.Id = id;
        _articleService.Publish(command);
        return Ok();
    }
}
