using Weblog.Application.Commands;
using Weblog.Application.ReadModels;

namespace Weblog.Application.Services;

public interface IArticleService
{
    string Create(CreateArticleCommand command);
    void Publish(PublishArticleCommand command);
    ArticleReadModel GetBySlug(string slug);
}
