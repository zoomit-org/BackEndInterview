using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Weblog.Application.Commands;
using Weblog.Application.ReadModels;
using Weblog.Domain;

namespace Weblog.Application.Services;

public class ArticleService : IArticleService
{
    private readonly IMongoDatabase _database;

    public ArticleService(IOptions<DatabaseOptions> databaseOptions)
    {
        var client = new MongoClient(databaseOptions.Value.ConnectionString);
        _database = client.GetDatabase(databaseOptions.Value.DatabaseName);
    }

    public string Create(CreateArticleCommand command)
    {
        var article = new Article
        {
            Title = command.Title,
            Content = command.Content,
            AuthorIds = command.AuthorIds,
            CreateDateTime = DateTime.UtcNow,
            CategoryId = command.CategoryId
        };

        var collection = _database.GetCollection<Article>("articles");
        collection.InsertOne(article);
        return article.Id;
    }

    public void Publish(PublishArticleCommand command)
    {
        var collection = _database.GetCollection<Article>("articles");
        var article = collection.Find(a => a.Id == command.Id).Single();

        if (article is null or { IsPublished: true })
        {
            throw new Exception();
        }

        article.PublishDateTime = command.PublishAt;

        if (article.PublishDateTime <= DateTime.UtcNow)
        {
            article.IsPublished = true;
        }

        collection.ReplaceOne(a => a.Id == article.Id, article);
    }

    public ArticleReadModel GetById(string id)
    {
        var article = _database.GetCollection<Article>("articles")
            .Find(a => a.Id == id)
            .Single();

        if (article is not { IsPublished: true })
        {
            throw new Exception();
        }

        var category = _database.GetCollection<Category>("categories")
            .Find(c => c.Id == article.CategoryId)
            .Single();

        var authors = new List<AuthorReadModel>(article.AuthorIds.Length);

        foreach (var authorId in article.AuthorIds)
        {
            var httpClient = new HttpClient();
            var response = httpClient.GetAsync($"http://iam/api/users/{authorId}").Result;
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
                Id = category.Id,
                Title = category.Title,
            },
            Authors = authors.ToArray(),
        };
    }
}
