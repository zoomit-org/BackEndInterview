using System;

namespace Weblog.Application.Commands;

public class PublishArticleCommand
{
    public string Id { get; set; }
    public DateTime PublishAt { get; set; }
}
