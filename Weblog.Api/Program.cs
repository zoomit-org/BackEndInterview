using Microsoft.EntityFrameworkCore;
using Weblog.Application;
using Weblog.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IArticleService, ArticleService>();
builder.Services.AddDbContext<DatabaseContext>(b => b.UseSqlServer(builder.Configuration["ConnectionString"]));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
