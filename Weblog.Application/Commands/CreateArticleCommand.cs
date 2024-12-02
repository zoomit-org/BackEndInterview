namespace Weblog.Application.Commands;

public class CreateArticleCommand
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string[] AuthorIds { get; set; }
    public int CategoryId { get; set; }
}
