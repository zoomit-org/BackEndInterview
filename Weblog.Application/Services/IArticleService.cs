using Weblog.Application.Commands;
using Weblog.Application.ReadModels;

namespace Weblog.Application.Services;

public interface IArticleService
{
    int Create(CreateArticleCommand command);
    void Publish(PublishArticleCommand command);
    ArticleReadModel GetById(int id);
}
