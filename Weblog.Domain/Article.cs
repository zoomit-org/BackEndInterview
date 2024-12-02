using System;

namespace Weblog.Domain;

public class Article
{
    public string Id { get; set; }

    public DateTime CreateDateTime { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishDateTime { get; set; }

    public string Title { get; set; }
    
    public string Slug { get; set; }
    
    public string Content { get; set; }
    
    public ArticleAuthor[] Authors { get; set; }

    public int CategoryId { get; set; }
    
    public Category Category { get; set; }
}
