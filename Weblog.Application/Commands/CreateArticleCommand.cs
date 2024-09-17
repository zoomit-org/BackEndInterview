namespace Weblog.Application.Commands;

public class CreateArticleCommand
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string[] AuthorIds { get; set; }
    public string CategoryId { get; set; }
}
