using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Weblog.Application.Commands;
using Weblog.Application.ReadModels;
using Weblog.Domain;

namespace Weblog.Application.Services;

public class ArticleService : IArticleService
{
    private readonly DatabaseContext _databaseContext;

    public ArticleService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public int Create(CreateArticleCommand command)
    {
        var article = new Article
        {
            Title = command.Title,
            Content = command.Content,
            Authors = command.AuthorIds.Select(id => new ArticleAuthor
                {
                    UserId = id
                })
                .ToArray(),
            CreateDateTime = DateTime.UtcNow,
            CategoryId = command.CategoryId
        };

        _databaseContext.Add(article);
        _databaseContext.SaveChanges();

        return article.Id;
    }

    public void Publish(PublishArticleCommand command)
    {
        var article = _databaseContext.Articles.Find(command.Id);

        if (article is null or { IsPublished: true })
        {
            throw new Exception();
        }

        article.PublishDateTime = command.PublishAt;

        if (article.PublishDateTime <= DateTime.UtcNow)
        {
            article.IsPublished = true;
        }

        _databaseContext.SaveChanges();
    }

    public ArticleReadModel GetById(int id)
    {
        var article = _databaseContext.Articles.AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.Authors)
            .Single(a => a.Id == id);

        if (article is null or { IsPublished: false })
        {
            throw new Exception();
        }

        var authors = new List<AuthorReadModel>(article.Authors.Length);

        foreach (var userId in article.Authors.Select(a => a.UserId))
        {
            var httpClient = new HttpClient();
            var response = httpClient.GetAsync($"http://iam/api/users/{userId}").Result;
            response.EnsureSuccessStatusCode();
            var author = response.Content.ReadFromJsonAsync<AuthorReadModel>().Result;
            authors.Add(author);
        }

        return new ArticleReadModel
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            PublishDateTime = article.PublishDateTime!.Value,
            Category = new CategoryReadModel
            {
                Id = article.Category.Id,
                Title = article.Category.Title,
            },
            Authors = authors.ToArray(),
        };
    }
}
