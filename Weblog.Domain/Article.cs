using System;

namespace Weblog.Domain;

public class Article
{
    public string Id { get; set; }

    public DateTime CreateDateTime { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishDateTime { get; set; }

    public string Title { get; set; }
    
    public string Content { get; set; }
    
    public string[] AuthorIds { get; set; }

    public string CategoryId { get; set; }
}
