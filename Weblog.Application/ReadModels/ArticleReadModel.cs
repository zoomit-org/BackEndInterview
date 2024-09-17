using System;

namespace Weblog.Application.ReadModels;

public class ArticleReadModel
{
    public string Id { get; set; }

    public DateTime PublishDateTime { get; set; }

    public string Title { get; set; }
    
    public string Content { get; set; }
    
    public AuthorReadModel[] Authors { get; set; }

    public CategoryReadModel Category { get; set; }
}
