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

    public string Create(CreateArticleCommand command)
    {
        if (_databaseContext.Articles.Any(a => a.Slug == command.Slug))
        {
            throw new Exception();
        }
        
        var article = new Article
        {
            Id = Guid.NewGuid().ToString(),
            Title = command.Title,
            Slug = command.Slug,
            Content = command.Content,
            Authors = command.AuthorIds.Select(id => new ArticleAuthor
                {
                    UserId = id,
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

    public ArticleReadModel GetBySlug(string slug)
    {
        var article = _databaseContext.Articles.AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.Authors)
            .Single(a => a.Slug == slug);

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

        var likesCountResponse = new HttpClient().GetAsync($"http://discussion/api/likes/{article.Id}").Result;
        likesCountResponse.EnsureSuccessStatusCode();

        return new ArticleReadModel
        {
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            Content = article.Content,
            PublishDateTime = article.PublishDateTime!.Value,
            Category = new CategoryReadModel
            {
                Id = article.Category.Id,
                Title = article.Category.Title,
            },
            Authors = authors.ToArray(),
            LikesCount = likesCountResponse.Content.ReadFromJsonAsync<int>().Result,
        };
    }
}
